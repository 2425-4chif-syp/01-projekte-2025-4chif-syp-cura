using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicationReminderController : ControllerBase
{
    private readonly ILogger<MedicationReminderController> _logger;
    private readonly IMqttService _mqttService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public MedicationReminderController(
        ILogger<MedicationReminderController> logger,
        IMqttService mqttService,
        IUnitOfWork unitOfWork,
        IConfiguration configuration)
    {
        _logger = logger;
        _mqttService = mqttService;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    /// <summary>
    /// Sendet eine Test-Nachricht an das Display-Topic
    /// </summary>
    [HttpPost("test-message")]
    public async Task<IActionResult> SendTestMessage([FromBody] string message = "Bitte Medikamente einnehmen - TEST")
    {
        try
        {
            var displayTopic = _configuration["Mqtt:Topics:DisplayMessage"] ?? "display/message";
            await _mqttService.PublishAsync(displayTopic, message);
            
            _logger.LogInformation("Test-Nachricht gesendet: {Message}", message);
            
            return Ok(new { 
                Success = true, 
                Message = $"Nachricht erfolgreich an Topic '{displayTopic}' gesendet",
                SentMessage = message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Senden der Test-Nachricht");
            return StatusCode(500, new { 
                Success = false, 
                Message = "Fehler beim Senden der Nachricht",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Überprüft aktuelle Medikamentenpläne und deren Zeiten
    /// </summary>
    [HttpGet("active-plans")]
    public async Task<IActionResult> GetActivePlans()
    {
        try
        {
            var activePlans = await _unitOfWork.MedicationPlans.GetActiveAsync();
            var now = DateTime.Now;
            
            var planInfo = activePlans.Select(plan => new
            {
                Id = plan.Id,
                PatientName = plan.Patient?.Name,
                MedicationName = plan.Medication?.Name,
                Quantity = plan.Quantity,
                WeekdayFlags = plan.WeekdayFlags,
                DayTimeFlags = plan.DayTimeFlags,
                ValidFrom = plan.ValidFrom,
                ValidTo = plan.ValidTo,
                IsActive = plan.IsActive,
                WeekdayNames = GetWeekdayNames(plan.WeekdayFlags),
                DayTimeNames = GetDayTimeNames(plan.DayTimeFlags),
                NextReminders = GetNextReminders(plan, now)
            }).ToList();

            return Ok(new { 
                Success = true,
                CurrentTime = now,
                TotalActivePlans = activePlans.Count,
                Plans = planInfo
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Abrufen der aktiven Pläne");
            return StatusCode(500, new { 
                Success = false, 
                Message = "Fehler beim Abrufen der Pläne",
                Error = ex.Message
            });
        }
    }

    private List<string> GetWeekdayNames(int weekdayFlags)
    {
        var days = new List<string>();
        var weekdays = new Dictionary<int, string>
        {
            { 1, "Sonntag" },
            { 2, "Montag" },
            { 4, "Dienstag" },
            { 8, "Mittwoch" },
            { 16, "Donnerstag" },
            { 32, "Freitag" },
            { 64, "Samstag" }
        };

        foreach (var kvp in weekdays)
        {
            if ((weekdayFlags & kvp.Key) != 0)
                days.Add(kvp.Value);
        }

        return days;
    }

    private List<string> GetDayTimeNames(int dayTimeFlags)
    {
        var times = new List<string>();
        var dayTimes = new Dictionary<int, string>
        {
            { 1, "Morgens (08:00)" },
            { 2, "Mittags (12:00)" },
            { 4, "Nachmittags (16:00)" },
            { 8, "Abends (20:00)" }
        };

        foreach (var kvp in dayTimes)
        {
            if ((dayTimeFlags & kvp.Key) != 0)
                times.Add(kvp.Value);
        }

        return times;
    }

    private List<string> GetNextReminders(MedicationPlan plan, DateTime now)
    {
        var reminders = new List<string>();
        var dayTimes = new Dictionary<int, TimeSpan>
        {
            { 1, new TimeSpan(8, 0, 0) },   // Morning
            { 2, new TimeSpan(12, 0, 0) },  // Noon
            { 8, new TimeSpan(20, 0, 0) },  // Evening
            { 16, new TimeSpan(22, 0, 0) }  // Night
        };

        var weekdays = new Dictionary<DayOfWeek, int>
        {
            { DayOfWeek.Sunday, 1 },
            { DayOfWeek.Monday, 2 },
            { DayOfWeek.Tuesday, 4 },
            { DayOfWeek.Wednesday, 8 },
            { DayOfWeek.Thursday, 16 },
            { DayOfWeek.Friday, 32 },
            { DayOfWeek.Saturday, 64 }
        };

        // Prüfe heute und die nächsten 6 Tage
        for (int i = 0; i < 7; i++)
        {
            var checkDate = now.Date.AddDays(i);
            var dayOfWeek = checkDate.DayOfWeek;
            var weekdayFlag = weekdays[dayOfWeek];

            // Prüfe ob dieser Wochentag aktiv ist
            if ((plan.WeekdayFlags & weekdayFlag) != 0)
            {
                // Prüfe jede Tageszeit
                foreach (var dayTime in dayTimes)
                {
                    if ((plan.DayTimeFlags & dayTime.Key) != 0)
                    {
                        var reminderDateTime = checkDate.Add(dayTime.Value);
                        
                        // Nur zukünftige Erinnerungen anzeigen
                        if (reminderDateTime > now)
                        {
                            reminders.Add($"{reminderDateTime:dd.MM.yyyy HH:mm}");
                        }
                    }
                }
            }
        }

        return reminders.Take(5).ToList(); // Maximal 5 nächste Erinnerungen
    }
}