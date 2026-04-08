using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Utils;

namespace Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        const string FILENAME = "electionresults.csv";

        private readonly ApplicationDbContext _dbContext = new ApplicationDbContext();

        public UnitOfWork() : this(new ApplicationDbContext())
        { }

        private UnitOfWork(ApplicationDbContext context)
        {
            _dbContext = new ApplicationDbContext();
            CityRepository = new CityRepository(_dbContext);
            ElectionResultRepository = new ElectionResultRepository(_dbContext);
            PartyRepository = new PartyRepository(_dbContext);
        }

        public UnitOfWork(IConfiguration configuration) : this(new ApplicationDbContext(configuration))
        { }
        public ICityRepository CityRepository { get; }

        public IElectionResultRepository ElectionResultRepository { get; }

        public IPartyRepository PartyRepository { get; }

        public async Task<int> SaveChangesAsync()
        {
            var entities = _dbContext!.ChangeTracker.Entries()
                .Where(entity => entity.State == EntityState.Added
                                 || entity.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToArray();  // Geänderte Entities ermitteln

            // Allfällige Validierungen der geänderten Entities durchführen
            foreach (var entity in entities)
            {
                ValidateEntity(entity);
            }
            return await _dbContext.SaveChangesAsync();

        }

        private void ValidateEntity(object entity)
        {

        }

        public async Task DeleteDatabaseAsync() => await _dbContext!.Database.EnsureDeletedAsync();
        public async Task MigrateDatabaseAsync() => await _dbContext!.Database.MigrateAsync();
        public async Task CreateDatabaseAsync() => await _dbContext!.Database.EnsureCreatedAsync();

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (disposing)
            {
                await _dbContext.DisposeAsync();
            }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task FillDbAsync()
        {
            await this.DeleteDatabaseAsync();
            await this.MigrateDatabaseAsync();

            List<ElectionResult> electionResults;
            List<Party> parties;
            List<City> cities;

            string[][] csvFile = MyFile.ReadStringMatrixFromCsv(FILENAME, true);

            cities = csvFile.GroupBy(line => new { CityCode = line[0], CityName = line[1], ElegibleVoters = line[2] }).Select(grp =>
                new City
                {
                    CityCode = grp.Key.CityCode,
                    CityName = grp.Key.CityName,
                    ElegibleVoters = Convert.ToInt32(grp.Key.ElegibleVoters)
                }).ToList();

            parties = csvFile.GroupBy(line=>line[3]).Select(grp =>
                new Party
                {
                    PartyName = grp.Key
                }).ToList();

            electionResults = csvFile.Select(line =>
                new ElectionResult()
                {

                    NrOfVotes = Convert.ToInt32(line[4]),
                    Party = parties.Single(p=>p.PartyName==line[3]),
                    City = cities.Single(c=>c.CityCode==line[0] && c.CityName==line[1])
                }).ToList();

            //Korrektur der Wahlkartenstädte (diese haben in der CSV keine Anzahl der Wahlberechtigten) -> aber den gleichen Städtecode wie die normalen Stimmen
            foreach (var city in cities.Where(c=>c.ElegibleVoters==0))
            {
                city.ElegibleVoters = cities.Where(c => c.CityCode == city.CityCode).OrderByDescending(c => c.ElegibleVoters).FirstOrDefault()?.ElegibleVoters??0;
            }

            _dbContext.Cities.AddRange(cities);    //in diesem Fall nicht nötig -> jede Stadt kommt in mindestens einem ElectionResult vor
            _dbContext.Parties.AddRange(parties);  //in diesem Fall nicht nötig -> jede Stadt kommt in mindestens einem ElectionResult vor
            
            _dbContext.ElectionResults.AddRange(electionResults);

            await SaveChangesAsync();
        }
    }

   
}
