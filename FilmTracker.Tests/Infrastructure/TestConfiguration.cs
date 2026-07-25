using Microsoft.Extensions.Configuration;

namespace FilmTracker.Tests.Infrastructure;

internal static class TestConfiguration
{
    private const string ConnectionStringName = "DefaultConnection";

    private static IConfiguration Configuration { get; } =
        new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

    public static string GetConnectionString()
    {
        return Configuration.GetConnectionString(ConnectionStringName)
               ?? throw new InvalidOperationException(
                   $"Connection string '{ConnectionStringName}' was not found.");
    }
}