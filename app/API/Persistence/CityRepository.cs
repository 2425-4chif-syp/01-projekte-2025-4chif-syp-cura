
using Core.Contracts;
using Core.Dtos;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class CityRepository : ICityRepository
    {
        private ApplicationDbContext _dbContext;

        public CityRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }



        public async Task<List<CityWithVotesDto>> GetAllWithVotes()
        {
            return await _dbContext.ElectionResults.GroupBy(er => er.City)
                .Select(grp => new CityWithVotesDto()
                {
                    Id = grp.Key!.Id,
                    CityName = grp.Key.CityName,
                    CityCode = grp.Key.CityCode,
                    ElegibleVoters = grp.Key.ElegibleVoters,
                    TotalVotes = grp.Sum(er => er.NrOfVotes)
                }).OrderBy(c=>c.CityName).ToListAsync();

            //return await _dbContext.Cities.Select(c => new CityWithVotesDto()
            //{
            //    CityCode = c.CityCode,
            //    CityName = c.CityName,
            //    ElegibleVoters = c.ElegibleVoters,
            //    TotalVotes = _dbContext.ElectionResults.Where(er => er.City_Id == c.Id).Sum(er=>er.NrOfVotes)
            //}).ToListAsync();
        }

        public async Task<City?> GetByIdAsync(int id)
        {
            return await _dbContext.Cities.FirstOrDefaultAsync(c=>c.Id==id);
        }

        public async Task<int> GetCountAsync()
        {
            return await _dbContext.Cities.CountAsync();
        }

    }
}