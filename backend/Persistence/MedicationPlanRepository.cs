using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class MedicationPlanRepository : IMedicationPlanRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicationPlanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MedicationPlan>> GetAllAsync()
        {
            return await _context.MedicationPlans
                .OrderBy(m => m.PatientId)
                .ThenBy(m => m.ValidFrom)
                .ToListAsync();
        }

        public async Task<MedicationPlan?> GetByIdAsync(int id)
        {
            return await _context.MedicationPlans
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<MedicationPlan>> GetByPatientIdAsync(int patientId)
        {
            return await _context.MedicationPlans
                .Include(m => m.Medication)
                .Where(m => m.PatientId == patientId)
                .OrderBy(m => m.ValidFrom)
                .ToListAsync();
        }

        public async Task<List<MedicationPlan>> GetByMedicationIdAsync(int medicationId)
        {
            return await _context.MedicationPlans
                .Where(m => m.MedicationId == medicationId)
                .OrderBy(m => m.PatientId)
                .ToListAsync();
        }

        public async Task<List<MedicationPlan>> GetByCaregiverIdAsync(int caregiverId)
        {
            return await _context.MedicationPlans
                .Where(m => m.CaregiverId == caregiverId)
                .OrderBy(m => m.PatientId)
                .ToListAsync();
        }

        public async Task<List<MedicationPlan>> GetActiveAsync()
        {
            // Temporärer Fix: Verwende lokale Zeit für Vergleich
            var today = DateTime.Now.Date;
            return await _context.MedicationPlans
                .Include(m => m.Patient)
                .Include(m => m.Medication)
                .Include(m => m.Caregiver)
                .Where(m => m.IsActive == true)  // Erstmal nur IsActive prüfen
                .OrderBy(m => m.PatientId)
                .ToListAsync();
        }

        public async Task<List<MedicationPlan>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.MedicationPlans
                .Where(m => m.ValidFrom <= to && 
                           (m.ValidTo == null || m.ValidTo >= from))
                .OrderBy(m => m.PatientId)
                .ThenBy(m => m.ValidFrom)
                .ToListAsync();
        }

        public async Task<List<MedicationPlan>> GetByPatientWeekdayAndDayTimeAsync(int patientId, int weekdayFlag, int dayTimeFlag, DateTime date)
        {
            return await _context.MedicationPlans
                .Include(m => m.Medication)
                .Where(m => m.PatientId == patientId &&
                           m.IsActive == true &&
                           (m.WeekdayFlags & weekdayFlag) == weekdayFlag &&
                           (m.DayTimeFlags & dayTimeFlag) == dayTimeFlag &&
                           m.ValidFrom <= date &&
                           (m.ValidTo == null || m.ValidTo >= date))
                .OrderBy(m => m.MedicationId)
                .ToListAsync();
        }

        public async Task AddAsync(MedicationPlan medicationPlan)
        {
            await _context.MedicationPlans.AddAsync(medicationPlan);
        }

        public void Update(MedicationPlan medicationPlan)
        {
            _context.MedicationPlans.Update(medicationPlan);
        }

        public void Delete(MedicationPlan medicationPlan)
        {
            _context.MedicationPlans.Remove(medicationPlan);
        }
    }
}