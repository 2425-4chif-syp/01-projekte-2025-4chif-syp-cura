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
                .Where(mi => mi.PatientId == patientId)
                .OrderByDescending(mi => mi.IntakeDate)
                    .ThenByDescending(mi => mi.OpenedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationIntake>> GetByPatientAndDateRangeAsync(int patientId, DateOnly startDate, DateOnly endDate)
        {
            return await _context.MedicationIntakes
                .Include(mi => mi.Patient)
                .Where(mi => mi.PatientId == patientId 
                    && mi.IntakeDate >= startDate 
                    && mi.IntakeDate <= endDate)
                .OrderBy(mi => mi.IntakeDate)
                    .ThenBy(mi => mi.DayTimeFlag)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationIntake>> GetByPatientAndDateAsync(int patientId, DateOnly date)
        {
            return await _context.MedicationIntakes
                .Include(mi => mi.Patient)
                .Where(mi => mi.PatientId == patientId && mi.IntakeDate == date)
                .OrderBy(mi => mi.DayTimeFlag)
                .ToListAsync();
        }

        public async Task<bool> WasDrawerOpenedAsync(int patientId, DateOnly date, int dayTimeFlag)
        {
            return await _context.MedicationIntakes
                .AnyAsync(mi => mi.PatientId == patientId &&
                               mi.IntakeDate == date &&
                               mi.DayTimeFlag == dayTimeFlag);
        }

        public async Task<MedicationIntake> CreateAsync(MedicationIntake intake)
        {
            _context.MedicationIntakes.Add(intake);
            await _context.SaveChangesAsync();
            
            // Reload with navigation properties
            return await _context.MedicationIntakes
                .Include(mi => mi.Patient)
                .FirstAsync(mi => mi.Id == intake.Id);
        }

        public async Task<MedicationIntake?> GetByIdAsync(int id)
        {
            return await _context.MedicationIntakes
                .Include(mi => mi.Patient)
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
