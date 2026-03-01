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
        /// Validiert Wochentag, Tageszeit und erstellt Medication Intake Einträge.
        /// </summary>
        /// <param name="request">Scan-Daten mit ChipId und optional Event-Type</param>
        /// <returns></returns>
        [HttpPost("scan")]
        public async Task<IActionResult> RegisterScan([FromBody] RfidScanRequest request)
        {
            if (string.IsNullOrEmpty(request.ChipId))
                return BadRequest("ChipId is required");

            // 1. Chip in Datenbank suchen
            var chip = await _unitOfWork.RfidChipRepository.GetByChipIdAsync(request.ChipId);
            if (chip == null)
            {
                return Ok(new { 
                    Success = false, 
                    Error = "CHIP_NOT_FOUND",
                    Message = "RFID chip not found in database",
                    ChipId = request.ChipId
                });
            }

            // 2. Heutigen Wochentag prüfen
            var now = DateTime.Now;
            var today = now.DayOfWeek;
            var todayString = today.ToString().ToUpper();
            
            if (chip.Weekday.ToUpper() != todayString)
            {
                return BadRequest(new { 
                    Success = false,
                    Error = "WRONG_WEEKDAY",
                    Message = $"This chip is for {chip.Weekday}, but today is {todayString}",
                    ChipId = request.ChipId,
                    ExpectedWeekday = chip.Weekday,
                    ActualWeekday = todayString
                });
            }

            // 3. Wochentag-Flag berechnen (Sun=1, Mon=2, Tue=4, etc.)
            int weekdayFlag = today switch
            {
                DayOfWeek.Sunday => 1,
                DayOfWeek.Monday => 2,
                DayOfWeek.Tuesday => 4,
                DayOfWeek.Wednesday => 8,
                DayOfWeek.Thursday => 16,
                DayOfWeek.Friday => 32,
                DayOfWeek.Saturday => 64,
                _ => 0
            };

            // 4. Aktuelle Tageszeit ermitteln und Flag berechnen
            var hour = now.Hour;
            int dayTimeFlag;
            string dayTimeName;
            
            if (hour >= 6 && hour < 11)
            {
                dayTimeFlag = 1; // Morning
                dayTimeName = "MORNING";
            }
            else if (hour >= 11 && hour < 14)
            {
                dayTimeFlag = 2; // Noon
                dayTimeName = "NOON";
            }
            else if (hour >= 14 && hour < 18)
            {
                dayTimeFlag = 4; // Afternoon
                dayTimeName = "AFTERNOON";
            }
            else if (hour >= 18 && hour < 22)
            {
                dayTimeFlag = 8; // Evening
                dayTimeName = "EVENING";
            }
            else
            {
                return Ok(new { 
                    Success = false,
                    Error = "OUTSIDE_TIME_WINDOW",
                    Message = $"Current time {now:HH:mm} is outside medication time windows (22:00-06:00 night rest)",
                    CurrentTime = now.ToString("HH:mm")
                });
            }

            // 5. Medication Plans für diesen Patienten, Wochentag und Tageszeit finden
            var medicationPlans = await _unitOfWork.MedicationPlanRepository
                .GetByPatientWeekdayAndDayTimeAsync(chip.PatientId, weekdayFlag, dayTimeFlag, DateTime.SpecifyKind(now.Date, DateTimeKind.Utc));

            if (!medicationPlans.Any())
            {
                return Ok(new { 
                    Success = false,
                    Error = "NO_MEDICATIONS_SCHEDULED",
                    Message = $"No medications scheduled for {todayString} at {dayTimeName}",
                    ChipId = request.ChipId,
                    PatientId = chip.PatientId,
                    Weekday = todayString,
                    DayTime = dayTimeName
                });
            }

            // 6. Prüfen welche Medikamente noch nicht eingenommen wurden
            var recordedIntakes = new List<object>();
            var alreadyTaken = new List<object>();

            foreach (var plan in medicationPlans)
            {
                // Check if there's already an intake for this plan today
                var todayIntakes = await _unitOfWork.MedicationIntakeRepository
                    .GetByPatientAndDateAsync(chip.PatientId, DateOnly.FromDateTime(now.Date));
                
                bool hasAlreadyTaken = todayIntakes.Any(i => i.MedicationPlanId == plan.Id);

                if (hasAlreadyTaken)
                {
                    alreadyTaken.Add(new
                    {
                        MedicationPlanId = plan.Id,
                        MedicationName = plan.Medication?.Name ?? "Unknown"
                    });
                }
                else
                {
                    // 7. Neuen Intake erstellen
                    var intake = new MedicationIntake
                    {
                        PatientId = chip.PatientId,
                        MedicationPlanId = plan.Id,
                        IntakeTime = now.ToUniversalTime(),
                        Quantity = plan.Quantity,
                        RfidTag = request.ChipId,
                        Notes = $"Auto-recorded via RFID scan at {dayTimeName}"
                    };

                    var createdIntake = await _unitOfWork.MedicationIntakeRepository.CreateAsync(intake);

                    recordedIntakes.Add(new
                    {
                        MedicationPlanId = plan.Id,
                        MedicationName = plan.Medication?.Name ?? "Unknown",
                        Quantity = plan.Quantity,
                        IntakeTime = createdIntake.IntakeTime
                    });
                }
            }

            // 8. Erfolgreiche Response
            if (recordedIntakes.Any())
            {
                return Ok(new { 
                    Success = true,
                    Message = $"Medications recorded successfully for {dayTimeName}",
                    Timestamp = now,
                    ChipId = request.ChipId,
                    PatientId = chip.PatientId,
                    Weekday = todayString,
                    DayTime = dayTimeName,
                    RecordedIntakes = recordedIntakes,
                    AlreadyTaken = alreadyTaken.Any() ? alreadyTaken : null
                });
            }
            else
            {
                return Ok(new { 
                    Success = false,
                    Error = "ALREADY_TAKEN",
                    Message = "All medications for this time period already recorded today",
                    ChipId = request.ChipId,
                    PatientId = chip.PatientId,
                    Weekday = todayString,
                    DayTime = dayTimeName,
                    AlreadyTaken = alreadyTaken
                });
            }
        }
    }
}

public class RfidScanRequest
{
    public string ChipId { get; set; } = string.Empty;
    public string Event { get; set; } = "scan"; // "scan" oder "removed"
}
