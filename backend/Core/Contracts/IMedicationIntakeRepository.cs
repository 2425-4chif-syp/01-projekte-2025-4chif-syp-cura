using Core.Entities;

namespace Core.Contracts
{
    public interface IMedicationIntakeRepository
    {
        /// <summary>
        /// Gets all medication intakes for a specific patient
        /// </summary>
        Task<IEnumerable<MedicationIntake>> GetByPatientIdAsync(int patientId);
        
        /// <summary>
        /// Gets all medication intakes for a patient within a date range
        /// </summary>
        Task<IEnumerable<MedicationIntake>> GetByPatientAndDateRangeAsync(int patientId, DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// Gets all medication intakes for a specific date
        /// </summary>
        Task<IEnumerable<MedicationIntake>> GetByPatientAndDateAsync(int patientId, DateTime date);
        
        /// <summary>
        /// Creates a new medication intake log
        /// </summary>
        Task<MedicationIntake> CreateAsync(MedicationIntake intake);
        
        /// <summary>
        /// Gets a specific medication intake by ID
        /// </summary>
        Task<MedicationIntake?> GetByIdAsync(int id);
        
        /// <summary>
        /// Updates an existing medication intake
        /// </summary>
        Task UpdateAsync(MedicationIntake intake);
        
        /// <summary>
        /// Deletes a medication intake
        /// </summary>
        Task DeleteAsync(int id);
    }
}
