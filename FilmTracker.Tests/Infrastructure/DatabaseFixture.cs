using FilmTracker.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace FilmTracker.Tests.Infrastructure;

public sealed class DatabaseFixture : IAsyncLifetime
{
    public AppDbContext CreateContext()
    {
        var connectionString = TestConfiguration.GetConnectionString();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    public async Task ClearDatabaseAsync()
    {
        await using var context = CreateContext();
        await context.Movies.ExecuteDeleteAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}