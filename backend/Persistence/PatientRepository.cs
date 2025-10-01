using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Patient>> GetAllAsync()
        {
            return await _context.Patients
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Patient>> GetByNameAsync(string name)
        {
            return await _context.Patients
                .Where(p => p.Name.Contains(name))
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Patient?> GetByEmailAsync(string email)
        {
            return await _context.Patients
                .Include(p => p.Location)
                .FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task AddAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
        }

        public void Update(Patient patient)
        {
            _context.Patients.Update(patient);
        }

        public void Delete(Patient patient)
        {
            _context.Patients.Remove(patient);
        }
    }
}