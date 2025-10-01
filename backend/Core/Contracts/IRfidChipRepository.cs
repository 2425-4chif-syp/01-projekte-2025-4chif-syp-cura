using Core.Entities;

namespace Core.Contracts
{
    public interface IRfidChipRepository
    {
        Task<List<RfidChip>> GetAllAsync();
        Task<RfidChip?> GetByIdAsync(int id);
        Task<RfidChip?> GetByChipIdAsync(string chipId);
        Task<List<RfidChip>> GetByWeekdayAsync(string weekday);
        Task AddAsync(RfidChip rfidChip);
        void Update(RfidChip rfidChip);
        void Delete(RfidChip rfidChip);
    }
}
