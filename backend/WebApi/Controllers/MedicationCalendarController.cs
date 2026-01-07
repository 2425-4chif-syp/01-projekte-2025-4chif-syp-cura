using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicationCalendarController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MedicationCalendarController> _logger;

    public MedicationCalendarController(IUnitOfWork unitOfWork, ILogger<MedicationCalendarController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get calendar overview for a specific month showing which days have all medications taken (true/false)
    /// </summary>
    /// <param name="patientId">The patient ID</param>
    /// <param name="year">Year (e.g., 2025)</param>
    /// <param name="month">Month (1-12)</param>
    /// <returns>Array of days with completion status</returns>
    [HttpGet("month/{patientId}/{year}/{month}")]
    public async Task<IActionResult> GetMonthCalendar(int patientId, int year, int month)
    {
        try
        {
            if (month < 1 || month > 12)
                return BadRequest("Month must be between 1 and 12");

            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(patientId);
            if (patient == null)
                return NotFound($"Patient with ID {patientId} not found");

            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // Get all medication plans for the patient in this month
            var plans = await _unitOfWork.MedicationPlanRepository.GetByPatientIdAsync(patientId);
            var activePlans = plans.Where(p => 
                p.IsActive && 
                p.ValidFrom.Date <= endDate &&
                (!p.ValidTo.HasValue || p.ValidTo.Value.Date >= startDate)
            ).ToList();

            // Get all intakes for this month
            var intakes = await _unitOfWork.MedicationIntakeRepository
                .GetByPatientAndDateRangeAsync(patientId, DateOnly.FromDateTime(startDate), DateOnly.FromDateTime(endDate));

            var intakesByDate = intakes.GroupBy(i => i.IntakeTime.Date).ToDictionary(g => g.Key, g => g.ToList());

            // Build calendar data for each day
            var calendarDays = new List<object>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var dayOfWeek = (int)date.DayOfWeek; // Sunday = 0, Monday = 1, etc.
                
                // Get expected medications for this day
                var expectedMedications = GetExpectedMedicationsForDay(activePlans, date, dayOfWeek);
                
                // Get actual intakes for this day
                var dayIntakes = intakesByDate.ContainsKey(date) ? intakesByDate[date] : new List<MedicationIntake>();
                
                // Check if all expected medications were taken
                var allTaken = CheckIfAllMedicationsTaken(expectedMedications, dayIntakes);
                
                calendarDays.Add(new
                {
                    date = date.ToString("yyyy-MM-dd"),
                    dayOfWeek = date.DayOfWeek.ToString(),
                    allMedicationsTaken = allTaken,
                    expectedCount = expectedMedications.Count,
                    takenCount = dayIntakes.Count
                });
            }

            return Ok(new
            {
                patientId,
                patientName = patient.Name,
                year,
                month,
                monthName = startDate.ToString("MMMM"),
                days = calendarDays
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting month calendar for patient {PatientId}", patientId);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get detailed information for a specific day
    /// </summary>
    /// <param name="patientId">The patient ID</param>
    /// <param name="date">Date in format yyyy-MM-dd</param>
    [HttpGet("day/{patientId}/{date}")]
    public async Task<IActionResult> GetDayDetails(int patientId, string date)
    {
        try
        {
            if (!DateTime.TryParse(date, out var parsedDate))
                return BadRequest("Invalid date format. Use yyyy-MM-dd");

            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(patientId);
            if (patient == null)
                return NotFound($"Patient with ID {patientId} not found");

            // Get medication plans active on this date
            var plans = await _unitOfWork.MedicationPlanRepository.GetByPatientIdAsync(patientId);
            var dayOfWeek = (int)parsedDate.DayOfWeek;
            
            var activePlans = plans.Where(p => 
                p.IsActive && 
                p.ValidFrom.Date <= parsedDate &&
                (!p.ValidTo.HasValue || p.ValidTo.Value.Date >= parsedDate)
            ).ToList();

            var expectedMedications = GetExpectedMedicationsForDay(activePlans, parsedDate, dayOfWeek);

            // Get actual intakes for this day
            var intakes = await _unitOfWork.MedicationIntakeRepository
                .GetByPatientAndDateAsync(patientId, DateOnly.FromDateTime(parsedDate));

            // Build detailed information
            var medicationDetails = expectedMedications.Select(em => {
                var taken = intakes.FirstOrDefault(i => i.MedicationPlanId == em.PlanId);
                return new
                {
                    medicationPlanId = em.PlanId,
                    medicationName = em.MedicationName,
                    expectedQuantity = em.Quantity,
                    expectedTimes = em.DayTimes.ToString(),
                    wasTaken = taken != null,
                    actualIntakeTime = taken?.IntakeTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    actualQuantity = taken?.Quantity,
                    notes = taken?.Notes,
                    rfidTag = taken?.RfidTag
                };
            }).ToList();

            var allTaken = medicationDetails.All(m => m.wasTaken);

            return Ok(new
            {
                patientId,
                patientName = patient.Name,
                date = parsedDate.ToString("yyyy-MM-dd"),
                dayOfWeek = parsedDate.DayOfWeek.ToString(),
                allMedicationsTaken = allTaken,
                medications = medicationDetails,
                summary = new
                {
                    expected = medicationDetails.Count,
                    taken = medicationDetails.Count(m => m.wasTaken),
                    missing = medicationDetails.Count(m => !m.wasTaken)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting day details for patient {PatientId} on {Date}", patientId, date);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Log a medication intake
    /// </summary>
    [HttpPost("log-intake")]
    public async Task<IActionResult> LogIntake([FromBody] LogIntakeRequest request)
    {
        try
        {
            if (request.PatientId <= 0 || request.MedicationPlanId <= 0)
                return BadRequest("Invalid patient or medication plan ID");

            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(request.PatientId);
            if (patient == null)
                return NotFound($"Patient with ID {request.PatientId} not found");

            var plan = await _unitOfWork.MedicationPlanRepository.GetByIdAsync(request.MedicationPlanId);
            if (plan == null)
                return NotFound($"Medication plan with ID {request.MedicationPlanId} not found");

            var intake = new MedicationIntake
            {
                PatientId = request.PatientId,
                MedicationPlanId = request.MedicationPlanId,
                IntakeTime = request.IntakeTime ?? DateTime.UtcNow,
                Quantity = request.Quantity > 0 ? request.Quantity : plan.Quantity,
                Notes = request.Notes,
                RfidTag = request.RfidTag
            };

            var created = await _unitOfWork.MedicationIntakeRepository.CreateAsync(intake);

            return Ok(new
            {
                success = true,
                intake = new
                {
                    id = created.Id,
                    patientId = created.PatientId,
                    patientName = created.Patient?.Name,
                    medicationName = created.MedicationPlan?.Medication?.Name,
                    intakeTime = created.IntakeTime,
                    quantity = created.Quantity,
                    notes = created.Notes,
                    rfidTag = created.RfidTag
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging medication intake");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #region Helper Methods

    private class ExpectedMedication
    {
        public int PlanId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public List<string> DayTimes { get; set; } = new();
    }

    private List<ExpectedMedication> GetExpectedMedicationsForDay(List<MedicationPlan> plans, DateTime date, int dayOfWeek)
    {
        var expected = new List<ExpectedMedication>();

        foreach (var plan in plans)
        {
            // Check if this plan is valid for this date
            if (plan.ValidFrom.Date > date || (plan.ValidTo.HasValue && plan.ValidTo.Value.Date < date))
                continue;

            // Check weekday flags (Sunday=1, Monday=2, Tuesday=4, etc.)
            var dayFlag = 1 << dayOfWeek;
            if ((plan.WeekdayFlags & dayFlag) == 0)
                continue; // This plan doesn't apply to this weekday

            // Get the day times
            var dayTimes = new List<string>();
            if ((plan.DayTimeFlags & 1) != 0) dayTimes.Add("Morning");
            if ((plan.DayTimeFlags & 2) != 0) dayTimes.Add("Noon");
            if ((plan.DayTimeFlags & 4) != 0) dayTimes.Add("Afternoon");
            if ((plan.DayTimeFlags & 8) != 0) dayTimes.Add("Evening");

            expected.Add(new ExpectedMedication
            {
                PlanId = plan.Id,
                MedicationName = plan.Medication.Name,
                Quantity = plan.Quantity,
                DayTimes = dayTimes
            });
        }

        return expected;
    }

    private bool CheckIfAllMedicationsTaken(List<ExpectedMedication> expected, List<MedicationIntake> intakes)
    {
        if (expected.Count == 0)
            return true; // No medications expected, so "all" are taken

        if (intakes.Count == 0)
            return false; // Medications expected but none taken

        // Check if each expected medication has a corresponding intake
        foreach (var exp in expected)
        {
            if (!intakes.Any(i => i.MedicationPlanId == exp.PlanId))
                return false;
        }

        return true;
    }

    #endregion
}

public class LogIntakeRequest
{
    public int PatientId { get; set; }
    public int MedicationPlanId { get; set; }
    public DateTime? IntakeTime { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
    public string? RfidTag { get; set; }
}
