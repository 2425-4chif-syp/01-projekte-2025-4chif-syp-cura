using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public MedicationsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Gets all medications.
        /// </summary>
        /// <param name="name">Optional: Search filter for name</param>
        /// <param name="activeIngredient">Optional: Search filter for active ingredient</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMedications(string? name, string? activeIngredient)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var filteredByName = await _unitOfWork.MedicationRepository.GetByNameAsync(name);
                return Ok(filteredByName);
            }

            if (!string.IsNullOrEmpty(activeIngredient))
            {
                var filteredByIngredient = await _unitOfWork.MedicationRepository.GetByActiveIngredientAsync(activeIngredient);
                return Ok(filteredByIngredient);
            }

            var medications = await _unitOfWork.MedicationRepository.GetAllAsync();
            return Ok(medications);
        }

        /// <summary>
        /// Gets a medication by ID.
        /// </summary>
        /// <param name="id">Medication ID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedication(int id)
        {
            var medication = await _unitOfWork.MedicationRepository.GetByIdAsync(id);
            if (medication == null)
                return NotFound();

            return Ok(medication);
        }

        /// <summary>
        /// Creates a new medication.
        /// </summary>
        /// <param name="medication">Medication data</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateMedication([FromBody] Medication medication)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _unitOfWork.MedicationRepository.AddAsync(medication);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMedication), new { id = medication.Id }, medication);
        }

        /// <summary>
        /// Updates an existing medication.
        /// </summary>
        /// <param name="id">Medication ID</param>
        /// <param name="medication">New medication data</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedication(int id, [FromBody] Medication medication)
        {
            if (id != medication.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingMedication = await _unitOfWork.MedicationRepository.GetByIdAsync(id);
            if (existingMedication == null)
                return NotFound();

            _unitOfWork.MedicationRepository.Update(medication);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a medication.
        /// </summary>
        /// <param name="id">Medication ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedication(int id)
        {
            var medication = await _unitOfWork.MedicationRepository.GetByIdAsync(id);
            if (medication == null)
                return NotFound();

            _unitOfWork.MedicationRepository.Delete(medication);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}