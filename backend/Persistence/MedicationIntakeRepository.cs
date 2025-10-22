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
                    .ThenInclude(mp => mp.Medication)
                .Where(mi => mi.PatientId == patientId)
                .OrderByDescending(mi => mi.IntakeTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationIntake>> GetByPatientAndDateRangeAsync(int patientId, DateTime startDate, DateTime endDate)
        {
            return await _context.MedicationIntakes
                .Include(mi => mi.Patient)
                .Include(mi => mi.MedicationPlan)
                    .ThenInclude(mp => mp.Medication)
                .Where(mi => mi.PatientId == patientId 
                    && mi.IntakeTime.Date >= startDate.Date 
                    && mi.IntakeTime.Date <= endDate.Date)
                .OrderBy(mi => mi.IntakeTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationIntake>> GetByPatientAndDateAsync(int patientId, DateTime date)
        {
            return await _context.MedicationIntakes
                .Include(mi => mi.Patient)
                .Include(mi => mi.MedicationPlan)
                    .ThenInclude(mp => mp.Medication)
                .Where(mi => mi.PatientId == patientId && mi.IntakeTime.Date == date.Date)
                .OrderBy(mi => mi.IntakeTime)
                .ToListAsync();
        }

        public async Task<MedicationIntake> CreateAsync(MedicationIntake intake)
        {
            _context.MedicationIntakes.Add(intake);
            await _context.SaveChangesAsync();
            
            // Reload with navigation properties
            return await _context.MedicationIntakes
                .Include(mi => mi.Patient)
                .Include(mi => mi.MedicationPlan)
                    .ThenInclude(mp => mp.Medication)
                .FirstAsync(mi => mi.Id == intake.Id);
        }

        public async Task<MedicationIntake?> GetByIdAsync(int id)
        {
            return await _context.MedicationIntakes
                .Include(mi => mi.Patient)
                .Include(mi => mi.MedicationPlan)
                    .ThenInclude(mp => mp.Medication)
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
