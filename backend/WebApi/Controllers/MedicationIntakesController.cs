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
                Id = intake.Id,
                PatientId = intake.PatientId,
                MedicationPlanId = intake.MedicationPlanId,
                IntakeTime = intake.IntakeTime,
                Quantity = intake.Quantity,
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
                Id = intake.Id,
                PatientId = intake.PatientId,
                MedicationPlanId = intake.MedicationPlanId,
                IntakeTime = intake.IntakeTime,
                Quantity = intake.Quantity,
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

            if (request.Quantity <= 0)
                return BadRequest("Quantity must be greater than 0");

            var intake = new MedicationIntake
            {
                PatientId = request.PatientId,
                MedicationPlanId = request.MedicationPlanId,
                IntakeTime = request.IntakeTime ?? DateTime.Now,
                Quantity = request.Quantity,
                RfidTag = request.RfidTag,
                Notes = request.Notes
            };

            var created = await _unitOfWork.MedicationIntakeRepository.CreateAsync(intake);

            var result = new MedicationIntakeDetailDto
            {
                Id = created.Id,
                PatientId = created.PatientId,
                MedicationPlanId = created.MedicationPlanId,
                IntakeTime = created.IntakeTime,
                Quantity = created.Quantity,
                RfidTag = created.RfidTag,
                Notes = created.Notes
            };

            var dateStr = created.IntakeTime.ToString("yyyy-MM-dd");
            return CreatedAtAction(nameof(GetIntakesForDate), 
                new { patientId = created.PatientId, dateStr }, 
                result);
        }
    }

    public class CreateMedicationIntakeRequest
    {
        public int PatientId { get; set; }
        public int? MedicationPlanId { get; set; }
        public DateTime? IntakeTime { get; set; }
        public int Quantity { get; set; } = 1;
        public string? RfidTag { get; set; }
        public string? Notes { get; set; }
    }
}
