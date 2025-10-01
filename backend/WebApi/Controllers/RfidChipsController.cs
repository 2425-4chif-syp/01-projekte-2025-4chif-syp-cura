using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RfidChipsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RfidChipsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Liefert alle RFID Chips.
        /// </summary>
        /// <param name="weekday">Optional: Filter by weekday</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRfidChips([FromQuery] string? weekday = null)
        {
            if (!string.IsNullOrEmpty(weekday))
            {
                var filteredChips = await _unitOfWork.RfidChipRepository.GetByWeekdayAsync(weekday.ToUpper());
                return Ok(filteredChips);
            }

            var chips = await _unitOfWork.RfidChipRepository.GetAllAsync();
            return Ok(chips);
        }

        /// <summary>
        /// Liefert einen RFID Chip anhand der ID.
        /// </summary>
        /// <param name="id">ID des RFID Chips</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRfidChip(int id)
        {
            var chip = await _unitOfWork.RfidChipRepository.GetByIdAsync(id);
            if (chip == null)
                return NotFound();

            return Ok(chip);
        }

        /// <summary>
        /// Liefert einen RFID Chip anhand der Chip-ID.
        /// </summary>
        /// <param name="chipId">Chip-ID des RFID Chips</param>
        /// <returns></returns>
        [HttpGet("by-chip-id/{chipId}")]
        public async Task<IActionResult> GetRfidChipByChipId(string chipId)
        {
            var chip = await _unitOfWork.RfidChipRepository.GetByChipIdAsync(chipId);
            if (chip == null)
                return NotFound();

            return Ok(chip);
        }

        /// <summary>
        /// Erstellt einen neuen RFID Chip.
        /// </summary>
        /// <param name="rfidChip">RFID Chip-Daten</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateRfidChip([FromBody] RfidChip rfidChip)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _unitOfWork.RfidChipRepository.AddAsync(rfidChip);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRfidChip), new { id = rfidChip.Id }, rfidChip);
        }

        /// <summary>
        /// Aktualisiert einen bestehenden RFID Chip.
        /// </summary>
        /// <param name="id">ID des RFID Chips</param>
        /// <param name="rfidChip">Neue RFID Chip-Daten</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRfidChip(int id, [FromBody] RfidChip rfidChip)
        {
            if (id != rfidChip.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingChip = await _unitOfWork.RfidChipRepository.GetByIdAsync(id);
            if (existingChip == null)
                return NotFound();

            _unitOfWork.RfidChipRepository.Update(rfidChip);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Löscht einen RFID Chip.
        /// </summary>
        /// <param name="id">ID des RFID Chips</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRfidChip(int id)
        {
            var chip = await _unitOfWork.RfidChipRepository.GetByIdAsync(id);
            if (chip == null)
                return NotFound();

            _unitOfWork.RfidChipRepository.Delete(chip);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}
