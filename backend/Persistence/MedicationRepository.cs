using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class MedicationRepository : IMedicationRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Medication>> GetAllAsync()
        {
            return await _context.Medications.OrderBy(m => m.Name).ToListAsync();
        }

        public async Task<Medication?> GetByIdAsync(int id)
        {
            return await _context.Medications
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<Medication>> GetByNameAsync(string name)
        {
            return await _context.Medications
                .Where(m => m.Name.Contains(name))
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<List<Medication>> GetByActiveIngredientAsync(string activeIngredient)
        {
            return await _context.Medications
                .Where(m => m.ActiveIngredient != null && m.ActiveIngredient.Contains(activeIngredient))
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task AddAsync(Medication medication)
        {
            await _context.Medications.AddAsync(medication);
        }

        public void Update(Medication medication)
        {
            _context.Medications.Update(medication);
        }

        public void Delete(Medication medication)
        {
            _context.Medications.Remove(medication);
        }
    }
}