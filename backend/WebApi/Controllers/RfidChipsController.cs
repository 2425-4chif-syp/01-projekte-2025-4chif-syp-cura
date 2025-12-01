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

        /// <summary>
        /// Registriert einen RFID-Scan vom ESP32.
        /// </summary>
        /// <param name="request">Scan-Daten mit ChipId und optional Event-Type</param>
        /// <returns></returns>
        [HttpPost("scan")]
        public async Task<IActionResult> RegisterScan([FromBody] RfidScanRequest request)
        {
            if (string.IsNullOrEmpty(request.ChipId))
                return BadRequest("ChipId is required");

            var chip = await _unitOfWork.RfidChipRepository.GetByChipIdAsync(request.ChipId);
            
            // Log den Scan (kann später erweitert werden für weitere Logik)
            Console.WriteLine($"RFID Scan registered: {request.ChipId} - Event: {request.Event} - Time: {DateTime.UtcNow}");
            
            if (chip == null)
            {
                return Ok(new { 
                    Success = false, 
                    Message = "RFID Chip not found in database",
                    ChipId = request.ChipId,
                    Event = request.Event
                });
            }

            return Ok(new { 
                Success = true, 
                Message = "RFID Chip recognized",
                Chip = chip,
                Event = request.Event,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}

public class RfidScanRequest
{
    public string ChipId { get; set; } = string.Empty;
    public string Event { get; set; } = "scan"; // "scan" oder "removed"
}
