using Core.Contracts;

namespace Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            RfidChipRepository = new RfidChipRepository(_context);
            LocationRepository = new LocationRepository(_context);
            MedicationRepository = new MedicationRepository(_context);
            CaregiverRepository = new CaregiverRepository(_context);
            PatientRepository = new PatientRepository(_context);
            MedicationPlanRepository = new MedicationPlanRepository(_context);
        }

        public IRfidChipRepository RfidChipRepository { get; }
        public ILocationRepository LocationRepository { get; }
        public IMedicationRepository MedicationRepository { get; }
        public ICaregiverRepository CaregiverRepository { get; }
        public IPatientRepository PatientRepository { get; }
        public IMedicationPlanRepository MedicationPlanRepository { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}