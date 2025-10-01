using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class CaregiverRepository : ICaregiverRepository
    {
        private readonly ApplicationDbContext _context;

        public CaregiverRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Caregiver>> GetAllAsync()
        {
            return await _context.Caregivers
                .Include(c => c.Location)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Caregiver?> GetByIdAsync(int id)
        {
            return await _context.Caregivers
                .Include(c => c.Location)
                .Include(c => c.MedicationPlans)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Caregiver>> GetByNameAsync(string name)
        {
            return await _context.Caregivers
                .Include(c => c.Location)
                .Where(c => c.Name.Contains(name))
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Caregiver?> GetByEmailAsync(string email)
        {
            return await _context.Caregivers
                .Include(c => c.Location)
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task AddAsync(Caregiver caregiver)
        {
            await _context.Caregivers.AddAsync(caregiver);
        }

        public void Update(Caregiver caregiver)
        {
            _context.Caregivers.Update(caregiver);
        }

        public void Delete(Caregiver caregiver)
        {
            _context.Caregivers.Remove(caregiver);
        }
    }
}