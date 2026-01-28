using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationPlansController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public MedicationPlansController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Gets all medication plans.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMedicationPlans()
        {
            var plans = await _unitOfWork.MedicationPlanRepository.GetAllAsync();
            return Ok(plans);
        }

        /// <summary>
        /// Gets all patients with their medication plans (for plan selection).
        /// </summary>
        /// <returns></returns>
        [HttpGet("patients-with-plans")]
        public async Task<IActionResult> GetPatientsWithPlans()
        {
            var plans = await _unitOfWork.MedicationPlanRepository.GetAllAsync();
            
            // Gruppiere nach Patient
            var patientsWithPlans = plans
                .GroupBy(p => p.PatientId)
                .Select(g => new
                {
                    Id = g.Key,
                    Name = g.First().Patient?.Name ?? $"Patient {g.Key}",
                    PlanCount = g.Count(),
                    HasActivePlans = g.Any(p => p.IsActive)
                })
                .OrderBy(p => p.Id)
                .ToList();
            
            return Ok(patientsWithPlans);
        }

        /// <summary>
        /// Gets all active medication plans.
        /// </summary>
        /// <returns></returns>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveMedicationPlans()
        {
            var plans = await _unitOfWork.MedicationPlanRepository.GetActiveAsync();
            return Ok(plans);
        }

        /// <summary>
        /// Gets a medication plan by ID.
        /// </summary>
        /// <param name="id">Medication plan ID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicationPlan(int id)
        {
            var plan = await _unitOfWork.MedicationPlanRepository.GetByIdAsync(id);
            if (plan == null)
                return NotFound();

            return Ok(plan);
        }

        /// <summary>
        /// Gets all medication plans for a specific patient.
        /// </summary>
        /// <param name="patientId">Patient ID</param>
        /// <returns></returns>
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetMedicationPlansByPatient(int patientId)
        {
            var plans = await _unitOfWork.MedicationPlanRepository.GetByPatientIdAsync(patientId);
            return Ok(plans);
        }

        /// <summary>
        /// Gets all medication plans for a specific medication.
        /// </summary>
        /// <param name="medicationId">Medication ID</param>
        /// <returns></returns>
        [HttpGet("medication/{medicationId}")]
        public async Task<IActionResult> GetMedicationPlansByMedication(int medicationId)
        {
            var plans = await _unitOfWork.MedicationPlanRepository.GetByMedicationIdAsync(medicationId);
            return Ok(plans);
        }

        /// <summary>
        /// Gets all medication plans for a specific caregiver.
        /// </summary>
        /// <param name="caregiverId">Caregiver ID</param>
        /// <returns></returns>
        [HttpGet("caregiver/{caregiverId}")]
        public async Task<IActionResult> GetMedicationPlansByCaregiver(int caregiverId)
        {
            var plans = await _unitOfWork.MedicationPlanRepository.GetByCaregiverIdAsync(caregiverId);
            return Ok(plans);
        }

        /// <summary>
        /// Creates a new medication plan.
        /// </summary>
        /// <param name="dto">Medication plan data</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateMedicationPlan([FromBody] CreateMedicationPlanDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Map DTO to Entity
            var medicationPlan = new MedicationPlan
            {
                PatientId = dto.PatientId,
                MedicationId = dto.MedicationId,
                CaregiverId = dto.CaregiverId,
                WeekdayFlags = dto.WeekdayFlags,
                DayTimeFlags = dto.DayTimeFlags,
                Quantity = dto.Quantity,
                ValidFrom = dto.ValidFrom,
                ValidTo = dto.ValidTo,
                Notes = dto.Notes,
                IsActive = dto.IsActive
            };

            await _unitOfWork.MedicationPlanRepository.AddAsync(medicationPlan);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMedicationPlan), new { id = medicationPlan.Id }, medicationPlan);
        }

        /// <summary>
        /// Updates an existing medication plan.
        /// </summary>
        /// <param name="id">Medication plan ID</param>
        /// <param name="medicationPlan">New medication plan data</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicationPlan(int id, [FromBody] MedicationPlan medicationPlan)
        {
            if (id != medicationPlan.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingPlan = await _unitOfWork.MedicationPlanRepository.GetByIdAsync(id);
            if (existingPlan == null)
                return NotFound();

            _unitOfWork.MedicationPlanRepository.Update(medicationPlan);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a medication plan.
        /// </summary>
        /// <param name="id">Medication plan ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicationPlan(int id)
        {
            var plan = await _unitOfWork.MedicationPlanRepository.GetByIdAsync(id);
            if (plan == null)
                return NotFound();

            _unitOfWork.MedicationPlanRepository.Delete(plan);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}