using Core.Contracts;
using Core.Entities;

namespace WebApi.Services;

public class MedicationReminderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MedicationReminderService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // Überprüfung jede Minute
    
    // Definierte Zeiten für die day_time_flags
    private readonly Dictionary<int, TimeSpan> _dayTimes = new()
    {
        { 1, new TimeSpan(8, 0, 0) },   // Morning = 08:00
        { 2, new TimeSpan(11, 0, 0) },  // Noon = 11:00
        { 4, new TimeSpan(16, 0, 0) },  // Afternoon = 16:00
        { 8, new TimeSpan(20, 0, 0) }   // Evening = 20:00
    };

    // Wochentage-Mapping für weekday_flags
    private readonly Dictionary<DayOfWeek, int> _weekdays = new()
    {
        { DayOfWeek.Sunday, 1 },
        { DayOfWeek.Monday, 2 },
        { DayOfWeek.Tuesday, 4 },
        { DayOfWeek.Wednesday, 8 },
        { DayOfWeek.Thursday, 16 },
        { DayOfWeek.Friday, 32 },
        { DayOfWeek.Saturday, 64 }
    };

    private readonly HashSet<string> _sentReminders = new(); // Cache für bereits gesendete Erinnerungen

    public MedicationReminderService(
        IServiceProvider serviceProvider,
        ILogger<MedicationReminderService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MedicationReminderService gestartet");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckMedicationReminders();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Überprüfen der Medikamenten-Erinnerungen");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task CheckMedicationReminders()
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var mqttService = scope.ServiceProvider.GetRequiredService<IMqttService>();

        try
        {
            var now = DateTime.Now;
            var currentTime = now.TimeOfDay;
            var currentWeekday = now.DayOfWeek;
            var currentWeekdayFlag = _weekdays[currentWeekday];

            // Hole alle aktiven Medikamentenpläne
            var activePlans = await unitOfWork.MedicationPlans.GetActiveAsync();

            foreach (var plan in activePlans)
            {
                // Prüfe ob der Plan heute gültig ist (Wochentag)
                if ((plan.WeekdayFlags & currentWeekdayFlag) == 0)
                    continue;

                // Prüfe ob der Plan im gültigen Zeitraum ist
                if (now < plan.ValidFrom || (plan.ValidTo.HasValue && now > plan.ValidTo))
                    continue;

                // Prüfe jede definierte Tageszeit
                foreach (var dayTimeFlag in _dayTimes.Keys)
                {
                    // Prüfe ob dieser Zeitpunkt für diesen Plan aktiv ist
                    if ((plan.DayTimeFlags & dayTimeFlag) == 0)
                        continue;

                    var targetTime = _dayTimes[dayTimeFlag];
                    
                    // Prüfe ob die aktuelle Zeit der Zielzeit entspricht (±2 Minuten Toleranz)
                    var timeDifference = Math.Abs((currentTime - targetTime).TotalMinutes);
                    
                    if (timeDifference <= 2)
                    {
                        var reminderKey = $"{plan.Id}_{now:yyyy-MM-dd}_{dayTimeFlag}";
                        
                        // Prüfe ob bereits eine Erinnerung für heute und diese Zeit gesendet wurde
                        if (!_sentReminders.Contains(reminderKey))
                        {
                            await SendMedicationReminder(mqttService, plan, targetTime);
                            _sentReminders.Add(reminderKey);
                            
                            _logger.LogInformation(
                                "Medikamenten-Erinnerung gesendet für Patient: {PatientName}, Medikament: {MedicationName}, Zeit: {Time}",
                                plan.Patient?.Name ?? "Unbekannt",
                                plan.Medication?.Name ?? "Unbekannt",
                                targetTime.ToString(@"hh\:mm"));
                        }
                    }
                }
            }

            // Bereinige alte Erinnerungen (älter als heute)
            CleanupOldReminders(now);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Überprüfen der Medikamentenpläne");
        }
    }

    private async Task SendMedicationReminder(IMqttService mqttService, MedicationPlan plan, TimeSpan scheduledTime)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            
            var displayTopic = configuration["Mqtt:Topics:DisplayMessage"] ?? "display/message";
            var message = $"Bitte Medikamente einnehmen: {plan.Medication?.Name} ({plan.Quantity}x) für {plan.Patient?.Name}";
            
            await mqttService.PublishAsync(displayTopic, message);
            
            _logger.LogInformation(
                "MQTT-Nachricht gesendet - Topic: {Topic}, Message: {Message}",
                displayTopic, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Fehler beim Senden der MQTT-Nachricht für Medikamentenplan {PlanId}", 
                plan.Id);
        }
    }

    private void CleanupOldReminders(DateTime currentDate)
    {
        var currentDateKey = currentDate.ToString("yyyy-MM-dd");
        var toRemove = _sentReminders
            .Where(key => !key.Contains(currentDateKey))
            .ToList();

        foreach (var key in toRemove)
        {
            _sentReminders.Remove(key);
        }

        if (toRemove.Count > 0)
        {
            _logger.LogDebug("Bereinigt {Count} alte Erinnerungen", toRemove.Count);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MedicationReminderService wird beendet");
        await base.StopAsync(cancellationToken);
    }
}