using Core.Entities;

namespace Core.Contracts
{
    public interface ICaregiverRepository
    {
        Task<List<Caregiver>> GetAllAsync();
        Task<Caregiver?> GetByIdAsync(int id);
        Task<List<Caregiver>> GetByNameAsync(string name);
        Task<Caregiver?> GetByEmailAsync(string email);
        Task AddAsync(Caregiver caregiver);
        void Update(Caregiver caregiver);
        void Delete(Caregiver caregiver);
    }
}
