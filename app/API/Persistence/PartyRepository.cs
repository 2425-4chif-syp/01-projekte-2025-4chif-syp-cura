using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    internal class PartyRepository: IPartyRepository
    {
        private ApplicationDbContext _dbContext;

        public PartyRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Party>> GetAll()
        {
            return await _dbContext.Parties.OrderBy(p=>p.PartyName).ToListAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _dbContext.Parties.CountAsync();
        }
    }
}