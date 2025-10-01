using Core.Entities;

namespace Core.Contracts
{
    public interface IPatientRepository
    {
        Task<List<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task<List<Patient>> GetByNameAsync(string name);
        Task<Patient?> GetByEmailAsync(string email);
        Task AddAsync(Patient patient);
        void Update(Patient patient);
        void Delete(Patient patient);
    }
}
