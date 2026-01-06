using Core.Contracts;
using Core.Entities;
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

        /// <summary>
        /// Get all drawer opening logs for a specific date and patient
        /// Returns which times of day the drawer was opened
        /// </summary>
        [HttpGet("patient/{patientId}/date/{dateStr}")]
        public async Task<ActionResult<List<MedicationIntakeDetailDto>>> GetIntakesForDate(
            int patientId, 
            string dateStr)
        {
            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(patientId);
            if (patient == null)
                return NotFound($"Patient with ID {patientId} not found");

            if (!DateOnly.TryParse(dateStr, out var date))
                return BadRequest("Invalid date format. Use YYYY-MM-DD");

            var intakes = await _unitOfWork.MedicationIntakeRepository
                .GetByPatientAndDateAsync(patientId, date);

            var result = intakes.Select(intake => new MedicationIntakeDetailDto
            {
                IntakeDate = intake.IntakeDate,
                DayTimeFlag = intake.DayTimeFlag,
                TimeLabel = GetTimeLabelFromFlag(intake.DayTimeFlag),
                OpenedAt = intake.OpenedAt,
                RfidTag = intake.RfidTag,
                Notes = intake.Notes
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Get all drawer opening logs for a patient within a date range
        /// </summary>
        [HttpGet("patient/{patientId}/range")]
        public async Task<ActionResult<List<MedicationIntakeDetailDto>>> GetIntakesForRange(
            int patientId,
            [FromQuery] string startDate,
            [FromQuery] string endDate)
        {
            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(patientId);
            if (patient == null)
                return NotFound($"Patient with ID {patientId} not found");

            if (!DateOnly.TryParse(startDate, out var start))
                return BadRequest("Invalid start date format. Use YYYY-MM-DD");

            if (!DateOnly.TryParse(endDate, out var end))
                return BadRequest("Invalid end date format. Use YYYY-MM-DD");

            var intakes = await _unitOfWork.MedicationIntakeRepository
                .GetByPatientAndDateRangeAsync(patientId, start, end);

            var result = intakes.Select(intake => new MedicationIntakeDetailDto
            {
                IntakeDate = intake.IntakeDate,
                DayTimeFlag = intake.DayTimeFlag,
                TimeLabel = GetTimeLabelFromFlag(intake.DayTimeFlag),
                OpenedAt = intake.OpenedAt,
                RfidTag = intake.RfidTag,
                Notes = intake.Notes
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Create a new drawer opening log (called by RFID system)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MedicationIntakeDetailDto>> CreateIntake(
            [FromBody] CreateMedicationIntakeRequest request)
        {
            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(request.PatientId);
            if (patient == null)
                return NotFound($"Patient with ID {request.PatientId} not found");

            // Validate day time flag (must be 1, 2, 4, or 8)
            if (request.DayTimeFlag != 1 && request.DayTimeFlag != 2 
                && request.DayTimeFlag != 4 && request.DayTimeFlag != 8)
            {
                return BadRequest("DayTimeFlag must be 1 (Morning), 2 (Noon), 4 (Afternoon), or 8 (Evening)");
            }

            var intake = new MedicationIntake
            {
                PatientId = request.PatientId,
                IntakeDate = request.IntakeDate,
                DayTimeFlag = request.DayTimeFlag,
                OpenedAt = DateTime.UtcNow,
                RfidTag = request.RfidTag,
                Notes = request.Notes
            };

            var created = await _unitOfWork.MedicationIntakeRepository.CreateAsync(intake);

            var result = new MedicationIntakeDetailDto
            {
                IntakeDate = created.IntakeDate,
                DayTimeFlag = created.DayTimeFlag,
                TimeLabel = GetTimeLabelFromFlag(created.DayTimeFlag),
                OpenedAt = created.OpenedAt,
                RfidTag = created.RfidTag,
                Notes = created.Notes
            };

            return CreatedAtAction(nameof(GetIntakesForDate), 
                new { patientId = created.PatientId, dateStr = created.IntakeDate.ToString("yyyy-MM-dd") }, 
                result);
        }

        private static string GetTimeLabelFromFlag(int flag)
        {
            return flag switch
            {
                1 => "Morgen",
                2 => "Mittag",
                4 => "Nachmittag",
                8 => "Abend",
                _ => "Unknown"
            };
        }
    }

    public class CreateMedicationIntakeRequest
    {
        public int PatientId { get; set; }
        public DateOnly IntakeDate { get; set; }
        public int DayTimeFlag { get; set; }
        public string? RfidTag { get; set; }
        public string? Notes { get; set; }
    }
}
