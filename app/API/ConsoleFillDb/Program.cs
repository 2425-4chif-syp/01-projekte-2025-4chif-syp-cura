// See https://aka.ms/new-console-template for more information
using Persistence;

Console.WriteLine("Löschen und migrieren der Datenbank, Import der Daten aus dem csv-File.....");
using (UnitOfWork unitOfWork = new UnitOfWork())
{

    Console.WriteLine("Städte, Parteien und Wahlergebnisse werden eingelesen");
    using (UnitOfWork uow = new UnitOfWork())
    {
        await uow.FillDbAsync();
        int cntCities = await uow.CityRepository.GetCountAsync();
        int cntParties = await uow.PartyRepository.GetCountAsync();
        int cntElectionResults = await uow.ElectionResultRepository.GetCountAsync();
        Console.WriteLine(cntCities + " Städte eingelesen!");
        Console.WriteLine(cntParties + " Parteien eingelesen!");
        Console.WriteLine(cntElectionResults + " Wahlergebnisse eingelesen!");
    }
    Console.Write("Beenden mit Eingabetaste ...");
    Console.ReadLine();
}