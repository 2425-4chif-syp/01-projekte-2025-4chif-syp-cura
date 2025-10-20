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
        { 1, new TimeSpan(10, 20, 0) },   // Morning = 10:20
        { 2, new TimeSpan(15, 03, 0) },  // Noon = 14:55
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
    private readonly HashSet<string> _acknowledgedReminders = new(); // Cache für quittierte Erinnerungen (Tag_Datum_Zeit)

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

        // MQTT-Service für RFID-Scan-Events abonnieren
        await SetupMqttListener();

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

    private async Task SetupMqttListener()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var mqttService = scope.ServiceProvider.GetRequiredService<IMqttService>();
            
            // Event-Handler für MQTT-Nachrichten registrieren
            mqttService.MessageReceived += OnMqttMessageReceived;
            
            _logger.LogInformation("MQTT-Listener für RFID-Scans eingerichtet");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Einrichten des MQTT-Listeners");
        }
    }

    private void OnMqttMessageReceived(object? sender, (string Topic, string Message) eventArgs)
    {
        var (topic, message) = eventArgs;
        
        // Prüfe ob es ein RFID-Scan vom rc522/tag Topic ist
        if (topic == "rc522/tag" && !string.IsNullOrWhiteSpace(message))
        {
            _ = Task.Run(async () => await HandleRfidScan(message.Trim()));
        }
    }

    private async Task HandleRfidScan(string rfidTag)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var mqttService = scope.ServiceProvider.GetRequiredService<IMqttService>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var now = DateTime.Now;
            var currentTime = now.TimeOfDay;
            var today = now.ToString("yyyy-MM-dd");

            // Finde die nächstliegende Einnahmezeit die gerade aktiv sein könnte
            var closestTimeFlag = FindClosestActiveTimeFlag(currentTime);
            
            if (closestTimeFlag.HasValue)
            {
                // VEREINFACHT: Da wir keine direkte Patient-RFID Zuordnung haben,
                // quittieren wir alle Patienten für diese Zeit
                // In einer echten App würdest du hier die RFID->Patient Zuordnung auflösen
                
                var allActivePlans = await unitOfWork.MedicationPlans.GetActiveAsync();
                var patientsWithMedication = allActivePlans
                    .Where(p => (p.DayTimeFlags & closestTimeFlag.Value) != 0)
                    .Select(p => p.PatientId)
                    .Distinct()
                    .ToList();

                foreach (var patientId in patientsWithMedication)
                {
                    var ackKey = $"PATIENT_{patientId}_{today}_{closestTimeFlag}";
                    _acknowledgedReminders.Add(ackKey);
                }
                
                // Sende leere Nachricht an Display um es zu löschen
                var displayTopic = configuration["Mqtt:Topics:DisplayMessage"] ?? "display/message";
                await mqttService.PublishAsync(displayTopic, "");
                
                var timeString = _dayTimes[closestTimeFlag.Value].ToString(@"hh\:mm");
                _logger.LogInformation("RFID-Tag {RfidTag} gescannt - Erinnerungen für {Time} quittiert ({Count} Patienten)", 
                    rfidTag, timeString, patientsWithMedication.Count);
            }
            else
            {
                _logger.LogInformation("RFID-Tag {RfidTag} gescannt, aber keine aktive Einnahmezeit gefunden", rfidTag);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Verarbeiten des RFID-Scans: {RfidTag}", rfidTag);
        }
    }

    private int? FindClosestActiveTimeFlag(TimeSpan currentTime)
    {
        // Suche die nächstliegende Einnahmezeit (±30 Minuten Toleranz)
        var tolerance = TimeSpan.FromMinutes(30);
        
        foreach (var kvp in _dayTimes)
        {
            var timeDiff = Math.Abs((currentTime - kvp.Value).TotalMinutes);
            if (timeDiff <= tolerance.TotalMinutes)
            {
                return kvp.Key;
            }
        }
        
        return null; // Keine Einnahmezeit in der Nähe
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

            _logger.LogInformation("Medication Check - Current Time: {Time}, Weekday: {Weekday}, Flag: {Flag}", 
                currentTime.ToString(@"hh\:mm\:ss"), currentWeekday, currentWeekdayFlag);

            // Hole alle aktiven Medikamentenpläne
            var activePlans = await unitOfWork.MedicationPlans.GetActiveAsync();
            
            _logger.LogInformation("Found {Count} active medication plans", activePlans.Count);

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
                    
                    // Prüfe ob die aktuelle Zeit exakt der Zielzeit entspricht (auf die Minute genau)
                    var currentMinute = new TimeSpan(currentTime.Hours, currentTime.Minutes, 0);
                    
                    if (currentMinute == targetTime)
                    {
                        var reminderKey = $"{plan.Id}_{now:yyyy-MM-dd}_{dayTimeFlag}";
                        
                        // Prüfe ob bereits eine Erinnerung für heute und diese Zeit gesendet wurde
                        if (!_sentReminders.Contains(reminderKey))
                        {
                            // Prüfe ob diese Erinnerung bereits quittiert wurde (über Patienten-RFID)
                            var patientAckKey = $"PATIENT_{plan.PatientId}_{now:yyyy-MM-dd}_{dayTimeFlag}";
                            
                            if (!_acknowledgedReminders.Contains(patientAckKey))
                            {
                                await SendMedicationReminder(mqttService, plan, targetTime);
                                _sentReminders.Add(reminderKey);
                                
                                _logger.LogInformation(
                                    "Medikamenten-Erinnerung gesendet für Patient: {PatientName}, Medikament: {MedicationName}, Zeit: {Time}",
                                    plan.Patient?.Name ?? "Unbekannt",
                                    plan.Medication?.Name ?? "Unbekannt",
                                    targetTime.ToString(@"hh\:mm"));
                            }
                            else
                            {
                                _logger.LogDebug(
                                    "Erinnerung für Patient {PatientId} um {Time} bereits quittiert", 
                                    plan.PatientId, targetTime.ToString(@"hh\:mm"));
                            }
                        }
                    }
                }
            }

            // Bereinige alte Erinnerungen (älter als heute)
            CleanupOldReminders(now);
            CleanupOldScans(now);
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
            
            // Fallback auf IDs falls Navigation Properties nicht geladen
            var medicationName = plan.Medication?.Name ?? $"Medikament-ID {plan.MedicationId}";
            var patientName = plan.Patient?.Name ?? $"Patient-ID {plan.PatientId}";
            var message = $"Bitte Medikamente einnehmen"; // $"Bitte Medikamente einnehmen: {medicationName} ({plan.Quantity}x) für {patientName}"
            
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

    private void CleanupOldScans(DateTime currentDate)
    {
        var currentDateKey = currentDate.ToString("yyyy-MM-dd");
        var toRemove = _acknowledgedReminders
            .Where(key => !key.Contains(currentDateKey))
            .ToList();

        foreach (var key in toRemove)
        {
            _acknowledgedReminders.Remove(key);
        }

        if (toRemove.Count > 0)
        {
            _logger.LogDebug("Bereinigt {Count} alte Quittierungen", toRemove.Count);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MedicationReminderService wird beendet");
        await base.StopAsync(cancellationToken);
    }
}