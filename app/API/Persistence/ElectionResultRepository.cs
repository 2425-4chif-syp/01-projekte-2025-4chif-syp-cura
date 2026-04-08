using Core.Contracts;
using Core.Dtos;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;

namespace Persistence
{
    internal class ElectionResultRepository : IElectionResultRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ElectionResultRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddElectionResult(ElectionResult electionResult)
        {
           this._dbContext.ElectionResults.Add(electionResult);
        }

        public Task<ElectionResult?> GetByIdAsync(int id)
        {
            return _dbContext.ElectionResults.SingleOrDefaultAsync(er => er.Id == id);
        }

        public Task<int> GetCountAsync()
        {
            return _dbContext.ElectionResults.CountAsync();
        }

        public async Task<List<CityResultDto>> GetResultByCityId(int cityId)
        {
            List<CityResultDto> result = await _dbContext.ElectionResults.Where(er=>er.City_Id == cityId)
                .Select(er=>new CityResultDto()
                { 
                    Id = er.Id,
                    CityName = er.City!.CityName,
                    PartyName = er.Party!.PartyName,
                    NumberOfVotes = er.NrOfVotes
                }).OrderByDescending(cr=>cr.NumberOfVotes).ToListAsync();

            int totalVotes = result.Sum(r => r.NumberOfVotes);

            result.ForEach(r => r.PartyPercent = (double)r.NumberOfVotes / totalVotes * 100);
            return result;
        }

        public Task<WinnerPartyDto?> GetWinner()
        {
            //return _dbContext.ElectionResults.GroupBy(er => er.Party)
            //    .Select(grp => new
            //    {
            //        PartyName = grp.Key!.PartyName,
            //        NumberOfVotes = grp.Sum(er => er.NrOfVotes)
            //    }).OrderByDescending(res => res.NumberOfVotes)
            //    .Select(res=>new WinnerPartyDto() { PartyName=res.PartyName})
            //    .FirstOrDefaultAsync();

            //Kurzform:

            return _dbContext.ElectionResults.GroupBy(er => er.Party)
                .OrderByDescending(res => res.Sum(er=>er.NrOfVotes))
                .Select(res => new WinnerPartyDto() { PartyName = res.Key!.PartyName })
                .FirstOrDefaultAsync();
        }
    }
}