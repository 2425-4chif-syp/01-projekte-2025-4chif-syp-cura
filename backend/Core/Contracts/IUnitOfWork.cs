namespace Core.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IRfidChipRepository RfidChipRepository { get; }
        ILocationRepository LocationRepository { get; }
        IMedicationRepository MedicationRepository { get; }
        ICaregiverRepository CaregiverRepository { get; }
        IPatientRepository PatientRepository { get; }
        IMedicationPlanRepository MedicationPlanRepository { get; }
        IMedicationPlanRepository MedicationPlans { get; }

        Task<int> SaveChangesAsync();
    }
}