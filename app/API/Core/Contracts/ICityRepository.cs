
using Core.Dtos;
using Core.Entities;

namespace Core.Contracts
{
    public interface ICityRepository
    {
        Task<List<CityWithVotesDto>> GetAllWithVotes();
        Task<City?> GetByIdAsync(int id);
        Task<int> GetCountAsync();
    }
}
