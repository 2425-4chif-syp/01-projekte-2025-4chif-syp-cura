using Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationIntakesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public MedicationIntakesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("patient/{patientId}/date/{date}")]
        public async Task<ActionResult<List<MedicationIntakeDetailDto>>> GetIntakesForDate(
            int patientId, 
            DateTime date)
        {
            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(patientId);
            if (patient == null)
                return NotFound($"Patient with ID {patientId} not found");

            var dayOfWeek = (int)date.DayOfWeek;
            var weekdayFlag = dayOfWeek == 0 ? 1 : (1 << dayOfWeek);

            // Get active medication plans for this patient and weekday
            var medicationPlans = await _unitOfWork.MedicationPlans.GetByPatientIdAsync(patientId);
            var activePlans = medicationPlans
                .Where(p => p.IsActive 
                         && (p.WeekdayFlags & weekdayFlag) != 0
                         && p.ValidFrom <= date 
                         && (!p.ValidTo.HasValue || p.ValidTo >= date))
                .ToList();

            // Get actual intakes for this day
            var intakes = await _unitOfWork.MedicationIntakeRepository.GetByPatientIdAsync(patientId);
            var dayIntakes = intakes
                .Where(i => i.IntakeTime.Date == date.Date)
                .ToList();

            var result = new List<MedicationIntakeDetailDto>();

            // Define time slots
            var timeSlots = new[]
            {
                new { Flag = 1, Label = "Morgen" },
                new { Flag = 2, Label = "Mittag" },
                new { Flag = 4, Label = "Nachmittag" },
                new { Flag = 8, Label = "Abend" }
            };

            foreach (var timeSlot in timeSlots)
            {
                var plansForTime = activePlans
                    .Where(p => (p.DayTimeFlags & timeSlot.Flag) != 0)
                    .ToList();

                foreach (var plan in plansForTime)
                {
                    var medication = await _unitOfWork.MedicationRepository.GetByIdAsync(plan.MedicationId);
                    if (medication == null) continue;

                    // Check if there's an intake record for this plan
                    var intake = dayIntakes.FirstOrDefault(i => i.MedicationPlanId == plan.Id);

                    result.Add(new MedicationIntakeDetailDto
                    {
                        MedicationPlanId = plan.Id,
                        MedicationName = medication.Name,
                        TimeLabel = timeSlot.Label,
                        ScheduledTime = date.Date.AddHours(GetHourForTimeSlot(timeSlot.Flag)),
                        IsTaken = intake != null,
                        TakenAt = intake?.IntakeTime
                    });
                }
            }

            return Ok(result.OrderBy(r => r.ScheduledTime).ToList());
        }

        private int GetHourForTimeSlot(int flag)
        {
            return flag switch
            {
                1 => 8,   // Morgen: 08:00
                2 => 12,  // Mittag: 12:00
                4 => 16,  // Nachmittag: 16:00
                8 => 20,  // Abend: 20:00
                _ => 12
            };
        }
    }
}
