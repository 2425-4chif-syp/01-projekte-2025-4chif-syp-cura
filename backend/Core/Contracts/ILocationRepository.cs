using Core.Entities;

namespace Core.Contracts
{
    public interface ILocationRepository
    {
        Task<List<Location>> GetAllAsync();
        Task<Location?> GetByIdAsync(int id);
        Task<List<Location>> GetByCityAsync(string city);
        Task AddAsync(Location location);
        void Update(Location location);
        void Delete(Location location);
    }
}
