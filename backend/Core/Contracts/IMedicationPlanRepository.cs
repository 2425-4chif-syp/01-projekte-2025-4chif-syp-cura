using Core.Entities;

namespace Core.Contracts
{
    public interface IMedicationPlanRepository
    {
        Task<List<MedicationPlan>> GetAllAsync();
        Task<MedicationPlan?> GetByIdAsync(int id);
        Task<List<MedicationPlan>> GetByPatientIdAsync(int patientId);
        Task<List<MedicationPlan>> GetByMedicationIdAsync(int medicationId);
        Task<List<MedicationPlan>> GetByCaregiverIdAsync(int caregiverId);
        Task<List<MedicationPlan>> GetActiveAsync();
        Task<List<MedicationPlan>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task AddAsync(MedicationPlan medicationPlan);
        void Update(MedicationPlan medicationPlan);
        void Delete(MedicationPlan medicationPlan);
    }
}
