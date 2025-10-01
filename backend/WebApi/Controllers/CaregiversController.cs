using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaregiversController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CaregiversController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Gets all caregivers.
        /// </summary>
        /// <param name="name">Optional: Search filter for name</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCaregivers(string? name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var filteredCaregivers = await _unitOfWork.CaregiverRepository.GetByNameAsync(name);
                return Ok(filteredCaregivers);
            }

            var caregivers = await _unitOfWork.CaregiverRepository.GetAllAsync();
            return Ok(caregivers);
        }

        /// <summary>
        /// Gets a caregiver by ID.
        /// </summary>
        /// <param name="id">Caregiver ID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCaregiver(int id)
        {
            var caregiver = await _unitOfWork.CaregiverRepository.GetByIdAsync(id);
            if (caregiver == null)
                return NotFound();

            return Ok(caregiver);
        }

        /// <summary>
        /// Creates a new caregiver.
        /// </summary>
        /// <param name="caregiver">Caregiver data</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateCaregiver([FromBody] Caregiver caregiver)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _unitOfWork.CaregiverRepository.AddAsync(caregiver);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCaregiver), new { id = caregiver.Id }, caregiver);
        }

        /// <summary>
        /// Updates an existing caregiver.
        /// </summary>
        /// <param name="id">Caregiver ID</param>
        /// <param name="caregiver">New caregiver data</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCaregiver(int id, [FromBody] Caregiver caregiver)
        {
            if (id != caregiver.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingCaregiver = await _unitOfWork.CaregiverRepository.GetByIdAsync(id);
            if (existingCaregiver == null)
                return NotFound();

            _unitOfWork.CaregiverRepository.Update(caregiver);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a caregiver.
        /// </summary>
        /// <param name="id">Caregiver ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCaregiver(int id)
        {
            var caregiver = await _unitOfWork.CaregiverRepository.GetByIdAsync(id);
            if (caregiver == null)
                return NotFound();

            _unitOfWork.CaregiverRepository.Delete(caregiver);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}