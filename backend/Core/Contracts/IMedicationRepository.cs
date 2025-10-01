using Core.Entities;

namespace Core.Contracts
{
    public interface IMedicationRepository
    {
        Task<List<Medication>> GetAllAsync();
        Task<Medication?> GetByIdAsync(int id);
        Task<List<Medication>> GetByNameAsync(string name);
        Task<List<Medication>> GetByActiveIngredientAsync(string activeIngredient);
        Task AddAsync(Medication medication);
        void Update(Medication medication);
        void Delete(Medication medication);
    }
}
