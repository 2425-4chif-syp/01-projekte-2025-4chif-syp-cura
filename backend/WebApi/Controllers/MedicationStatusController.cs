using Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationStatusController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public MedicationStatusController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("patient/{patientId}/month/{year}/{month}")]
        public async Task<ActionResult<IEnumerable<DailyStatusDto>>> GetDailyStatus(
            int patientId, int year, int month)
        {
            if (month < 1 || month > 12)
                return BadRequest("Invalid month");

            if (year < 2000 || year > 2100)
                return BadRequest("Invalid year");

            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(patientId);
            if (patient == null)
                return NotFound($"Patient with ID {patientId} not found");

            var daysInMonth = DateTime.DaysInMonth(year, month);
            var result = new List<DailyStatusDto>();

            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, daysInMonth, 23, 59, 59);

            var medicationPlans = await _unitOfWork.MedicationPlans.GetByPatientIdAsync(patientId);
            var activePlans = medicationPlans
                .Where(p => p.IsActive 
                         && p.ValidFrom <= endDate 
                         && (!p.ValidTo.HasValue || p.ValidTo >= startDate))
                .ToList();

            var intakes = await _unitOfWork.MedicationIntakeRepository.GetByPatientIdAsync(patientId);
            var monthIntakes = intakes
                .Where(i => i.IntakeTime >= startDate && i.IntakeTime <= endDate)
                .ToList();

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day);
                var dayOfWeek = (int)date.DayOfWeek;
                var weekdayFlag = dayOfWeek == 0 ? 1 : (1 << dayOfWeek);

                // Determine which time slots are scheduled for this day
                var scheduledTimeSlots = new HashSet<int>(); // 1=Morning, 2=Noon, 4=Afternoon, 8=Evening
                
                foreach (var plan in activePlans)
                {
                    if ((plan.WeekdayFlags & weekdayFlag) != 0 
                        && plan.ValidFrom <= date 
                        && (!plan.ValidTo.HasValue || plan.ValidTo >= date))
                    {
                        // Add each time slot that this plan covers
                        if ((plan.DayTimeFlags & 1) != 0) scheduledTimeSlots.Add(1); // Morning
                        if ((plan.DayTimeFlags & 2) != 0) scheduledTimeSlots.Add(2); // Noon
                        if ((plan.DayTimeFlags & 4) != 0) scheduledTimeSlots.Add(4); // Afternoon
                        if ((plan.DayTimeFlags & 8) != 0) scheduledTimeSlots.Add(8); // Evening
                    }
                }

                var scheduledCount = scheduledTimeSlots.Count;

                // Count how many time slots have intakes (drawer was opened)
                var dayIntakes = monthIntakes.Where(i => i.IntakeTime.Date == date).ToList();
                var takenTimeSlots = new HashSet<int>();
                
                foreach (var intake in dayIntakes)
                {
                    var hour = intake.IntakeTime.Hour;
                    // Determine which time slot this intake belongs to
                    if (hour >= 6 && hour < 11) takenTimeSlots.Add(1);      // Morning
                    else if (hour >= 11 && hour < 14) takenTimeSlots.Add(2); // Noon
                    else if (hour >= 14 && hour < 18) takenTimeSlots.Add(4); // Afternoon
                    else if (hour >= 18 && hour < 24) takenTimeSlots.Add(8); // Evening
                }

                var takenCount = takenTimeSlots.Count;

                var status = scheduledCount == 0 ? "empty" 
                           : takenCount >= scheduledCount ? "checked"
                           : "missed";

                result.Add(new DailyStatusDto
                {
                    Date = date,
                    Scheduled = scheduledCount,
                    Taken = takenCount,
                    Status = status
                });
            }

            return Ok(result);
        }
    }
}
