using Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationAlertController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public MedicationAlertController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Prüft ob für einen Patienten gerade ein Medikament fällig ist (aber noch nicht genommen wurde).
        /// Wird vom Raspberry Pi ATEM-Controller verwendet.
        /// </summary>
        [HttpGet("should-alert/{patientId}")]
        public async Task<IActionResult> ShouldAlert(int patientId)
        {
            // Local time für Österreich (UTC+1 oder UTC+2 bei Sommerzeit)
            var austriaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Vienna");
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, austriaTimeZone);
            var today = now.Date;
            var hour = now.Hour;

            // Tageszeit ermitteln (Morning=1, Noon=2, Afternoon=4, Evening=8)
            int currentTimeSlot;
            string timeSlotName;
            
            if (hour >= 6 && hour < 11)
            {
                currentTimeSlot = 1; // Morning
                timeSlotName = "Morning";
            }
            else if (hour >= 11 && hour < 14)
            {
                currentTimeSlot = 2; // Noon
                timeSlotName = "Noon";
            }
            else if (hour >= 14 && hour < 18)
            {
                currentTimeSlot = 4; // Afternoon
                timeSlotName = "Afternoon";
            }
            else if (hour >= 18)
            {
                currentTimeSlot = 8; // Evening (18:00-23:59)
                timeSlotName = "Evening";
            }
            else
            {
                // Außerhalb der Medikamentenzeiten (00:00-06:00)
                return Ok(new
                {
                    ShouldAlert = false,
                    Reason = "Outside medication time windows (night rest)",
                    CurrentTime = now.ToString("HH:mm")
                });
            }

            // Wochentag-Flag berechnen
            var dayOfWeek = (int)now.DayOfWeek;
            var weekdayFlag = dayOfWeek == 0 ? 1 : (1 << dayOfWeek);

            // Medikamentenpläne für diesen Patienten, Wochentag und Tageszeit holen
            var medicationPlans = await _unitOfWork.MedicationPlanRepository
                .GetByPatientWeekdayAndDayTimeAsync(patientId, weekdayFlag, currentTimeSlot, DateTime.SpecifyKind(today, DateTimeKind.Utc));

            if (!medicationPlans.Any())
            {
                return Ok(new
                {
                    ShouldAlert = false,
                    Reason = "No medications scheduled for current time",
                    TimeSlot = timeSlotName
                });
            }

            // Prüfen ob bereits Einnahmen für heute UND diesen Zeitslot existieren
            var todayIntakes = await _unitOfWork.MedicationIntakeRepository
                .GetByPatientAndDateAsync(patientId, DateOnly.FromDateTime(today));

            // Einnahmen für diesen Zeitslot filtern (basierend auf IntakeTime)
            var timeSlotIntakes = todayIntakes.Where(intake =>
            {
                var intakeHour = intake.IntakeTime.ToLocalTime().Hour;
                
                // Zeitslot der Einnahme bestimmen
                if (intakeHour >= 6 && intakeHour < 11 && currentTimeSlot == 1) return true;
                if (intakeHour >= 11 && intakeHour < 14 && currentTimeSlot == 2) return true;
                if (intakeHour >= 14 && intakeHour < 18 && currentTimeSlot == 4) return true;
                if (intakeHour >= 18 && currentTimeSlot == 8) return true;
                
                return false;
            }).ToList();

            // Wenn noch keine Einnahme für diesen Zeitslot existiert → Alert!
            if (!timeSlotIntakes.Any())
            {
                return Ok(new
                {
                    ShouldAlert = true,
                    Reason = "Medication due but not yet taken",
                    TimeSlot = timeSlotName,
                    CurrentTime = now.ToString("HH:mm"),
                    ScheduledMedications = medicationPlans.Count(),
                    TakenMedications = 0,
                    Medications = medicationPlans.Select(p => new
                    {
                        Name = p.Medication?.Name ?? "Unknown",
                        Quantity = p.Quantity
                    })
                });
            }

            // Einnahmen vorhanden → kein Alert
            return Ok(new
            {
                ShouldAlert = false,
                Reason = "Medications already taken",
                TimeSlot = timeSlotName,
                CurrentTime = now.ToString("HH:mm"),
                ScheduledMedications = medicationPlans.Count(),
                TakenMedications = timeSlotIntakes.Count
            });
        }
    }
}
