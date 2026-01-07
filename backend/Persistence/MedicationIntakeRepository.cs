using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class MedicationIntakeRepository : IMedicationIntakeRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicationIntakeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MedicationIntake>> GetByPatientIdAsync(int patientId)
        {
            return await _context.MedicationIntakes
                .Include(mi => mi.Patient)
                .Include(mi => mi.MedicationPlan)
                .Where(mi => mi.PatientId == patientId)
                .OrderByDescending(mi => mi.IntakeTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationIntake>> GetByPatientAndDateRangeAsync(int patientId, DateOnly startDate, DateOnly endDate)
        {
            var startDateTime = DateTime.SpecifyKind(startDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            var endDateTime = DateTime.SpecifyKind(endDate.ToDateTime(TimeOnly.MaxValue), DateTimeKind.Utc);
            
            return await _context.MedicationIntakes
                .Include(mi => mi.Patient)
                .Include(mi => mi.MedicationPlan)
                .Where(mi => mi.PatientId == patientId 
                    && mi.IntakeTime >= startDateTime 
                    && mi.IntakeTime <= endDateTime)
                .OrderBy(mi => mi.IntakeTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationIntake>> GetByPatientAndDateAsync(int patientId, DateOnly date)
        {
            var startDateTime = DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            var endDateTime = DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MaxValue), DateTimeKind.Utc);
            
            return await _context.MedicationIntakes
                .Include(mi => mi.Patient)
                .Include(mi => mi.MedicationPlan)
                .Where(mi => mi.PatientId == patientId 
                    && mi.IntakeTime >= startDateTime 
                    && mi.IntakeTime <= endDateTime)
                .OrderBy(mi => mi.IntakeTime)
                .ToListAsync();
        }

        public async Task<bool> WasDrawerOpenedAsync(int patientId, DateOnly date, int dayTimeFlag)
        {
            // Note: dayTimeFlag logic moved to application layer since DB doesn't store it
            // This method now checks if ANY intake exists for the date
            var startDateTime = DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            var endDateTime = DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MaxValue), DateTimeKind.Utc);
            
            return await _context.MedicationIntakes
                .AnyAsync(mi => mi.PatientId == patientId 
                    && mi.IntakeTime >= startDateTime 
                    && mi.IntakeTime <= endDateTime);
        }

        public async Task<MedicationIntake> CreateAsync(MedicationIntake intake)
        {
            _context.MedicationIntakes.Add(intake);
            await _context.SaveChangesAsync();
            
            // Reload with navigation properties
            return await _context.MedicationIntakes
                .Include(mi => mi.Patient)
                .Include(mi => mi.MedicationPlan)
                .FirstAsync(mi => mi.Id == intake.Id);
        }

        public async Task<MedicationIntake?> GetByIdAsync(int id)
        {
            return await _context.MedicationIntakes
                .Include(mi => mi.Patient)
                .Include(mi => mi.MedicationPlan)
                .FirstOrDefaultAsync(mi => mi.Id == id);
        }

        public async Task UpdateAsync(MedicationIntake intake)
        {
            _context.MedicationIntakes.Update(intake);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var intake = await _context.MedicationIntakes.FindAsync(id);
            if (intake != null)
            {
                _context.MedicationIntakes.Remove(intake);
                await _context.SaveChangesAsync();
            }
        }
    }
}
