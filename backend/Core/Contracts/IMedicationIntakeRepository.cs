using Core.Entities;

namespace Core.Contracts
{
    public interface IMedicationIntakeRepository
    {
        /// <summary>
        /// Gets all drawer opening logs for a specific patient
        /// </summary>
        Task<IEnumerable<MedicationIntake>> GetByPatientIdAsync(int patientId);
        
        /// <summary>
        /// Gets all drawer opening logs for a patient within a date range
        /// </summary>
        Task<IEnumerable<MedicationIntake>> GetByPatientAndDateRangeAsync(int patientId, DateOnly startDate, DateOnly endDate);
        
        /// <summary>
        /// Gets all drawer opening logs for a specific date
        /// </summary>
        Task<IEnumerable<MedicationIntake>> GetByPatientAndDateAsync(int patientId, DateOnly date);
        
        /// <summary>
        /// Checks if drawer was opened at specific time of day for a date
        /// </summary>
        Task<bool> WasDrawerOpenedAsync(int patientId, DateOnly date, int dayTimeFlag);
        
        /// <summary>
        /// Creates a new drawer opening log
        /// </summary>
        Task<MedicationIntake> CreateAsync(MedicationIntake intake);
        
        /// <summary>
        /// Gets a specific drawer opening log by ID
        /// </summary>
        Task<MedicationIntake?> GetByIdAsync(int id);
        
        /// <summary>
        /// Updates an existing drawer opening log
        /// </summary>
        Task UpdateAsync(MedicationIntake intake);
        
        /// <summary>
        /// Deletes a drawer opening log
        /// </summary>
        Task DeleteAsync(int id);
    }
}
