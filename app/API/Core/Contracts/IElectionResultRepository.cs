using Core.Dtos;
using Core.Entities;

namespace Core.Contracts
{
    public interface IElectionResultRepository
    {
        void AddElectionResult(ElectionResult electionResult);
        Task<ElectionResult?> GetByIdAsync(int id);
        Task<int> GetCountAsync();
        Task<List<CityResultDto>> GetResultByCityId(int cityId);
        Task<WinnerPartyDto?> GetWinner();
    }
}
