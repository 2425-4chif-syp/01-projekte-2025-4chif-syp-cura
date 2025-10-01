using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Ruft alle Patienten ab.
        /// </summary>
        /// <param name="name">Optional: Suchfilter für den Namen</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPatients([FromQuery] string? name = null)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var filteredPatients = await _unitOfWork.PatientRepository.GetByNameAsync(name);
                return Ok(filteredPatients);
            }

            var patients = await _unitOfWork.PatientRepository.GetAllAsync();
            return Ok(patients);
        }

        /// <summary>
        /// Liefert einen Patient anhand der ID.
        /// </summary>
        /// <param name="id">ID des Patienten</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatient(int id)
        {
            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(id);
            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        /// <summary>
        /// Erstellt einen neuen Patienten.
        /// </summary>
        /// <param name="patient">Patient-Daten</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] Patient patient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _unitOfWork.PatientRepository.AddAsync(patient);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }

        /// <summary>
        /// Aktualisiert einen bestehenden Patienten.
        /// </summary>
        /// <param name="id">ID des Patienten</param>
        /// <param name="patient">Neue Patient-Daten</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] Patient patient)
        {
            if (id != patient.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingPatient = await _unitOfWork.PatientRepository.GetByIdAsync(id);
            if (existingPatient == null)
                return NotFound();

            _unitOfWork.PatientRepository.Update(patient);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Löscht einen Patienten.
        /// </summary>
        /// <param name="id">ID des Patienten</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(id);
            if (patient == null)
                return NotFound();

            _unitOfWork.PatientRepository.Delete(patient);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}
