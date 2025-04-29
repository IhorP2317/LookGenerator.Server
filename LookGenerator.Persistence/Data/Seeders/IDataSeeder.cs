namespace LookGenerator.Persistence.Data.Seeders;

public interface IDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
    void Seed();
}