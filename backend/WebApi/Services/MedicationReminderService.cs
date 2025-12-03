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
        { 1, new TimeSpan(9, 31, 0) },   // TEST: 09:20 CET
        { 2, new TimeSpan(9, 31, 0) },   // TEST: Alle gleich für Test
        { 4, new TimeSpan(9, 31, 0) },   // TEST: Alle gleich für Test
        { 8, new TimeSpan(9, 31, 0) }    // TEST: Alle gleich für Test
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
    
    // Tracking für Missed Medication Alerts
    private readonly Dictionary<string, DateTime> _missedMedicationAlerts = new(); // Key: ReminderKey, Value: Zeit der Erinnerung
    private readonly TimeSpan _alertDelay = TimeSpan.FromMinutes(2); // TEST: Nur 2 Minuten warten!

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
                await CheckForMissedMedications();
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
            // Verwende Central European Time (CET/CEST)
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
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
                                // ✅ WICHTIG: ERST zur Liste hinzufügen (für Alert-Tracking)
                                _sentReminders.Add(reminderKey);
                                
                                // Dann MQTT-Nachricht senden (kann fehlschlagen)
                                try
                                {
                                    await SendMedicationReminder(mqttService, plan, targetTime);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning(ex, "MQTT-Nachricht konnte nicht gesendet werden für Plan {PlanId}", plan.Id);
                                }
                                
                                _logger.LogInformation(
                                    "Medikamenten-Erinnerung getrackt für Patient: {PatientName}, Medikament: {MedicationName}, Zeit: {Time}",
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

    private async Task CheckForMissedMedications()
    {
        try
        {
            // Verwende Central European Time (CET/CEST)
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            var today = now.ToString("yyyy-MM-dd");
            
            // Finde alle Erinnerungen die noch nicht quittiert wurden
            var missedReminders = _sentReminders
                .Where(reminderKey => 
                {
                    // Parse reminderKey: "PlanId_Date_TimeFlag"
                    var parts = reminderKey.Split('_');
                    if (parts.Length < 3) return false;
                    
                    var planId = parts[0];
                    var date = parts[1];
                    var timeFlag = parts[2];
                    
                    // Nur heute
                    if (date != today) return false;
                    
                    // Prüfe ob diese Erinnerung bereits quittiert wurde
                    // Wir müssen die PatientId aus dem Plan holen
                    return true; // Erst mal alle nicht quittierten heute
                })
                .ToList();

            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            foreach (var reminderKey in missedReminders)
            {
                // Parse reminderKey: "PlanId_Date_TimeFlag"
                var parts = reminderKey.Split('_');
                if (parts.Length < 3) continue;
                
                var planId = int.Parse(parts[0]);
                var date = parts[1];
                var timeFlag = int.Parse(parts[2]);
                
                // Finde den MedicationPlan
                var allPlans = await unitOfWork.MedicationPlans.GetActiveAsync();
                var plan = allPlans.FirstOrDefault(p => p.Id == planId);
                
                if (plan == null) continue;
                
                // Prüfe ob Patient diese Zeit-Erinnerung quittiert hat
                var patientAckKey = $"PATIENT_{plan.PatientId}_{date}_{timeFlag}";
                
                if (_acknowledgedReminders.Contains(patientAckKey))
                {
                    // Wurde quittiert - entferne aus Tracking
                    _missedMedicationAlerts.Remove(reminderKey);
                    continue;
                }
                
                // Prüfe ob wir diese Erinnerung schon im Alert-Tracking haben
                if (!_missedMedicationAlerts.ContainsKey(reminderKey))
                {
                    // Erste Prüfung - füge zum Tracking hinzu
                    _missedMedicationAlerts[reminderKey] = now;
                    _logger.LogDebug("Tracking missed medication: {ReminderKey}", reminderKey);
                    continue;
                }
                
                // Prüfe ob genug Zeit vergangen ist für einen Alert
                var reminderTime = _missedMedicationAlerts[reminderKey];
                var timeSinceReminder = now - reminderTime;
                
                if (timeSinceReminder >= _alertDelay)
                {
                    // Zeit ist abgelaufen - sende Alert-E-Mail
                    // ✅ Lade Patient explizit aus DB (Navigation Properties könnten NULL sein)
                    var patient = await unitOfWork.PatientRepository.GetByIdAsync(plan.PatientId);
                    
                    if (patient == null)
                    {
                        _logger.LogWarning("Patient {PatientId} nicht gefunden", plan.PatientId);
                        _missedMedicationAlerts.Remove(reminderKey);
                        continue;
                    }
                    
                    var medicationName = plan.Medication?.Name ?? $"Medikament-ID {plan.MedicationId}";
                    var scheduledTime = _dayTimes[timeFlag];
                    
                    try
                    {
                        await emailService.SendMissedMedicationAlertAsync(
                            patient.Email ?? "timon.schmalzer@gmail.com",
                            patient.Name ?? "Unbekannter Patient",
                            medicationName,
                            scheduledTime
                        );
                        
                        _logger.LogWarning(
                            "Alert-E-Mail gesendet: Patient {PatientName} hat {MedicationName} um {Time} verpasst",
                            patient.Name, medicationName, scheduledTime);
                        
                        // Entferne aus beiden Trackings (nur einmal Alert senden)
                        _missedMedicationAlerts.Remove(reminderKey);
                        _sentReminders.Remove(reminderKey);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Fehler beim Senden der Alert-E-Mail für {ReminderKey}", reminderKey);
                    }
                }
            }
            
            // Cleanup alte Alerts (älter als heute)
            var oldAlerts = _missedMedicationAlerts
                .Where(kvp => !kvp.Key.Contains(today))
                .Select(kvp => kvp.Key)
                .ToList();
                
            foreach (var key in oldAlerts)
            {
                _missedMedicationAlerts.Remove(key);
            }
            
            if (oldAlerts.Count > 0)
            {
                _logger.LogDebug("Bereinigt {Count} alte Alert-Trackings", oldAlerts.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Überprüfen verpasster Medikamente");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MedicationReminderService wird beendet");
        await base.StopAsync(cancellationToken);
    }
}