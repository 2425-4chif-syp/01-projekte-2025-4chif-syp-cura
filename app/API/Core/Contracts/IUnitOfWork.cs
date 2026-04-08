namespace Core.Contracts
{
    public interface IUnitOfWork: IDisposable
    {
        ICityRepository CityRepository { get; }
        IElectionResultRepository ElectionResultRepository { get; }

        IPartyRepository PartyRepository { get; }
        Task<int> SaveChangesAsync();
        Task DeleteDatabaseAsync();
        Task MigrateDatabaseAsync();
        Task CreateDatabaseAsync();

        Task FillDbAsync();
    }
}
