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

                var scheduledCount = activePlans.Count(p => 
                    (p.WeekdayFlags & weekdayFlag) != 0 
                    && p.ValidFrom <= date 
                    && (!p.ValidTo.HasValue || p.ValidTo >= date));

                var takenCount = monthIntakes.Count(i => i.IntakeTime.Date == date);

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
