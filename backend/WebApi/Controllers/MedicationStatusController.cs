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

        [HttpGet("patient/{patientId}/day/{year}/{month}/{day}")]
        public async Task<ActionResult<IEnumerable<DayDetailDto>>> GetDayDetails(
            int patientId, int year, int month, int day)
        {
            var date = new DateTime(year, month, day);
            var dayOfWeek = (int)date.DayOfWeek;
            var weekdayFlag = dayOfWeek == 0 ? 1 : (1 << dayOfWeek);

            // Get all active plans for this patient
            var medicationPlans = await _unitOfWork.MedicationPlans.GetByPatientIdAsync(patientId);
            var activePlans = medicationPlans
                .Where(p => p.IsActive 
                         && p.ValidFrom <= date
                         && (!p.ValidTo.HasValue || p.ValidTo >= date)
                         && (p.WeekdayFlags & weekdayFlag) != 0)
                .ToList();

            // Get actual intakes for this day
            var intakes = await _unitOfWork.MedicationIntakeRepository.GetByPatientIdAsync(patientId);
            var dayIntakes = intakes
                .Where(i => i.IntakeTime.Date == date)
                .ToList();

            var result = new List<DayDetailDto>();

            // Process each plan
            foreach (var plan in activePlans)
            {
                // Check each time slot
                for (int timeFlag = 1; timeFlag <= 8; timeFlag *= 2)
                {
                    if ((plan.DayTimeFlags & timeFlag) != 0)
                    {
                        // This medication is scheduled for this time slot
                        var timeLabel = timeFlag switch
                        {
                            1 => "Morning",
                            2 => "Noon",
                            4 => "Afternoon",
                            8 => "Evening",
                            _ => "Unknown"
                        };

                        // Find if there's an intake for this plan in this time slot
                        var intake = dayIntakes.FirstOrDefault(i => 
                            i.MedicationPlanId == plan.Id && 
                            GetTimeSlotFromHour(i.IntakeTime.Hour) == timeFlag);

                        result.Add(new DayDetailDto
                        {
                            MedicationPlanId = plan.Id,
                            MedicationName = plan.Medication?.Name ?? "Unknown",
                            TimeLabel = timeLabel,
                            TimeSlotFlag = timeFlag,
                            DayTimeFlag = timeFlag,
                            Quantity = plan.Quantity,
                            WasTaken = intake != null,
                            IntakeTime = intake?.IntakeTime,
                            ActualQuantity = intake?.Quantity,
                            Notes = intake?.Notes
                        });
                    }
                }
            }

            return Ok(result.OrderBy(r => r.TimeSlotFlag).ThenBy(r => r.MedicationName));
        }

        private int GetTimeSlotFromHour(int hour)
        {
            if (hour >= 6 && hour < 11) return 1;      // Morning
            else if (hour >= 11 && hour < 14) return 2; // Noon
            else if (hour >= 14 && hour < 18) return 4; // Afternoon
            else if (hour >= 18 && hour < 24) return 8; // Evening
            return 0;
        }
    }
}
