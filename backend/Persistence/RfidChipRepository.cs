using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class RfidChipRepository : IRfidChipRepository
    {
        private readonly ApplicationDbContext _context;

        public RfidChipRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RfidChip>> GetAllAsync()
        {
            return await _context.RfidChips.OrderBy(r => r.Weekday).ToListAsync();
        }

        public async Task<RfidChip?> GetByIdAsync(int id)
        {
            return await _context.RfidChips.FindAsync(id);
        }

        public async Task<RfidChip?> GetByChipIdAsync(string chipId)
        {
            return await _context.RfidChips.FirstOrDefaultAsync(r => r.ChipId == chipId);
        }

        public async Task<List<RfidChip>> GetByWeekdayAsync(string weekday)
        {
            return await _context.RfidChips
                .Where(r => r.Weekday == weekday)
                .ToListAsync();
        }

        public async Task AddAsync(RfidChip rfidChip)
        {
            await _context.RfidChips.AddAsync(rfidChip);
        }

        public void Update(RfidChip rfidChip)
        {
            _context.RfidChips.Update(rfidChip);
        }

        public void Delete(RfidChip rfidChip)
        {
            _context.RfidChips.Remove(rfidChip);
        }
    }
}
