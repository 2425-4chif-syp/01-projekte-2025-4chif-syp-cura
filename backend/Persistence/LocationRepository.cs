using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ApplicationDbContext _context;

        public LocationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Location>> GetAllAsync()
        {
            return await _context.Locations
                .OrderBy(l => l.City)
                .ThenBy(l => l.Street)
                .ToListAsync();
        }

        public async Task<Location?> GetByIdAsync(int id)
        {
            return await _context.Locations
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<Location>> GetByCityAsync(string city)
        {
            return await _context.Locations
                .Where(l => l.City.Contains(city))
                .OrderBy(l => l.Street)
                .ToListAsync();
        }

        public async Task AddAsync(Location location)
        {
            await _context.Locations.AddAsync(location);
        }

        public void Update(Location location)
        {
            _context.Locations.Update(location);
        }

        public void Delete(Location location)
        {
            _context.Locations.Remove(location);
        }
    }
}