using CuraApi.Data;
using CuraApi.Dtos;
using CuraApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuraApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicationController : ControllerBase
{
    private readonly CuraDbContext _db;

    public MedicationController(CuraDbContext db)
    {
        _db = db;
    }

    // GET api/medication/patients
    [HttpGet("patients")]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatients()
    {
        var patients = await _db.Patients
            .Select(p => new PatientDto(p.Id, p.Name, p.Age, p.PhoneNumber, p.Email))
            .ToListAsync();
        return Ok(patients);
    }

    // GET api/medication/patients/1
    [HttpGet("patients/{id}")]
    public async Task<ActionResult<PatientDto>> GetPatient(int id)
    {
        var patient = await _db.Patients.FindAsync(id);
        if (patient == null) return NotFound();
        return Ok(new PatientDto(patient.Id, patient.Name, patient.Age, patient.PhoneNumber, patient.Email));
    }

    // GET api/medication/plans/patient/1
    [HttpGet("plans/patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<MedicationPlanDto>>> GetPlansByPatient(int patientId)
    {
        var plans = await _db.MedicationPlans
            .Include(mp => mp.Patient)
            .Include(mp => mp.Medication)
            .Where(mp => mp.PatientId == patientId && mp.IsActive)
            .Select(mp => new MedicationPlanDto(
                mp.Id,
                mp.PatientId,
                mp.Patient.Name,
                mp.MedicationId,
                mp.Medication.Name,
                mp.WeekdayFlags,
                mp.DayTimeFlags,
                mp.Quantity,
                mp.ValidFrom,
                mp.ValidTo,
                mp.Notes,
                mp.IsActive
            ))
            .ToListAsync();
        return Ok(plans);
    }

    // GET api/medication/intakes/patient/1/month/2026/2
    [HttpGet("intakes/patient/{patientId}/month/{year}/{month}")]
    public async Task<ActionResult<IEnumerable<MedicationIntakeDto>>> GetIntakesByMonth(
        int patientId, int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var intakes = await _db.MedicationIntakes
            .Include(mi => mi.Patient)
            .Include(mi => mi.MedicationPlan)
                .ThenInclude(mp => mp.Medication)
            .Where(mi => mi.PatientId == patientId 
                      && mi.IntakeTime >= startDate 
                      && mi.IntakeTime <= endDate)
            .Select(mi => new MedicationIntakeDto(
                mi.Id,
                mi.PatientId,
                mi.Patient.Name,
                mi.MedicationPlanId,
                mi.MedicationPlan.Medication.Name,
                mi.IntakeTime,
                mi.Quantity,
                mi.Notes,
                mi.RfidTag
            ))
            .ToListAsync();
        return Ok(intakes);
    }

    // GET api/medication/status/patient/1/month/2026/2
    [HttpGet("status/patient/{patientId}/month/{year}/{month}")]
    public async Task<ActionResult<IEnumerable<DailyStatusDto>>> GetDailyStatus(
        int patientId, int year, int month)
    {
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var startDate = new DateTime(year, month, 1);
        var endDate = new DateTime(year, month, daysInMonth, 23, 59, 59);

        var plans = await _db.MedicationPlans
            .Where(p => p.PatientId == patientId 
                     && p.IsActive 
                     && p.ValidFrom <= endDate 
                     && (!p.ValidTo.HasValue || p.ValidTo >= startDate))
            .ToListAsync();

        var intakes = await _db.MedicationIntakes
            .Where(i => i.PatientId == patientId 
                     && i.IntakeTime >= startDate 
                     && i.IntakeTime <= endDate)
            .ToListAsync();

        var result = new List<DailyStatusDto>();

        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(year, month, day);
            var dayOfWeek = (int)date.DayOfWeek;
            var weekdayFlag = dayOfWeek == 0 ? 1 : (1 << dayOfWeek);

            var scheduledCount = plans.Count(p => 
                (p.WeekdayFlags & weekdayFlag) != 0 
                && p.ValidFrom <= date 
                && (!p.ValidTo.HasValue || p.ValidTo >= date));

            var takenCount = intakes.Count(i => i.IntakeTime.Date == date);

            var status = scheduledCount == 0 ? "empty" 
                       : takenCount >= scheduledCount ? "checked"
                       : "missed";

            result.Add(new DailyStatusDto(date, scheduledCount, takenCount, status));
        }

        return Ok(result);
    }

    // POST api/medication/intake
    [HttpPost("intake")]
    public async Task<ActionResult<MedicationIntakeDto>> LogIntake([FromBody] LogIntakeRequest request)
    {
        var patient = await _db.Patients.FindAsync(request.PatientId);
        if (patient == null) return NotFound($"Patient {request.PatientId} not found");

        var plan = await _db.MedicationPlans
            .Include(p => p.Medication)
            .FirstOrDefaultAsync(p => p.Id == request.MedicationPlanId);
        if (plan == null) return NotFound($"MedicationPlan {request.MedicationPlanId} not found");

        var intake = new MedicationIntake
        {
            PatientId = request.PatientId,
            MedicationPlanId = request.MedicationPlanId,
            IntakeTime = request.IntakeTime ?? DateTime.UtcNow,
            Quantity = request.Quantity > 0 ? request.Quantity : plan.Quantity,
            Notes = request.Notes,
            RfidTag = request.RfidTag
        };

        _db.MedicationIntakes.Add(intake);
        await _db.SaveChangesAsync();

        return Ok(new MedicationIntakeDto(
            intake.Id,
            intake.PatientId,
            patient.Name,
            intake.MedicationPlanId,
            plan.Medication.Name,
            intake.IntakeTime,
            intake.Quantity,
            intake.Notes,
            intake.RfidTag
        ));
    }
}
