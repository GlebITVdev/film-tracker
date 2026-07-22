using FilmTracker.Core.Data;
using FilmTracker.Core.Models;
using FilmTracker.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FilmTracker.Tests;

public class MovieRepositoryTests : IAsyncLifetime
{
    private static readonly IConfiguration Configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

    private AppDbContext CreateContext()
    {
        var connectionString = Configuration.GetConnectionString("DefaultConnection");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }

    private async Task SetupDatabaseAsync()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();
        await context.Movies.ExecuteDeleteAsync();
    }

    public async Task InitializeAsync() => await SetupDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task AddAsync_ShouldSaveCorrectData_WhenMovieIsAdded()
    {
        await using var context = CreateContext();
        var repository = new MovieRepository(context);
        var movie = new Movie("Inception", MovieStatus.ToWatch);

        await repository.AddAsync(movie);

        await using var assertContext = CreateContext();
        var addedMovie = await assertContext.Movies.SingleAsync();
        Assert.Equal(movie, addedMovie);
    }

    [Fact]
    public async Task GetByStatusAsync_ShouldReturnOnlyToWatchMovies_WhenStatusIsToWatch()
    {
        await using var context = CreateContext();
        await using var seedContext = CreateContext();

        var expected = new[]
        {
            new Movie("Inception", MovieStatus.ToWatch),
            new Movie("Interstellar", MovieStatus.ToWatch)
        };

        seedContext.Movies.AddRange(expected);
        seedContext.Movies.Add(new Movie("The Dark Knight", MovieStatus.Watched));
        await seedContext.SaveChangesAsync();

        var repository = new MovieRepository(context);
        var movies = await repository.GetByStatusAsync(MovieStatus.ToWatch);

        Assert.Equal(
            expected.OrderBy(movie => movie.Title),
            movies.OrderBy(movie => movie.Title));
    }

    [Fact]
    public async Task GetByStatusAsync_ShouldReturnOnlyWatchedMovies_WhenStatusIsWatched()
    {
        await using var context = CreateContext();
        await using var seedContext = CreateContext();

        var expected = new[]
        {
            new Movie("Interstellar", MovieStatus.Watched),
            new Movie("The Dark Knight", MovieStatus.Watched)
        };

        seedContext.Movies.Add(new Movie("Inception", MovieStatus.ToWatch));
        seedContext.Movies.AddRange(expected);
        await seedContext.SaveChangesAsync();

        var repository = new MovieRepository(context);
        var movies = await repository.GetByStatusAsync(MovieStatus.Watched);

        Assert.Equal(
            expected.OrderBy(movie => movie.Title),
            movies.OrderBy(movie => movie.Title));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllMovies()
    {
        await using var context = CreateContext();
        await using var seedContext = CreateContext();

        var expected = new[]
        {
            new Movie("Inception", MovieStatus.ToWatch),
            new Movie("The Dark Knight", MovieStatus.Watched)
        };

        seedContext.Movies.AddRange(expected);
        await seedContext.SaveChangesAsync();

        var repository = new MovieRepository(context);
        var movies = await repository.GetAllAsync();

        Assert.Equal(
            expected.OrderBy(movie => movie.Title),
            movies.OrderBy(movie => movie.Title));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenNoMoviesExist()
    {
        await using var context = CreateContext();
        var repository = new MovieRepository(context);

        var movies = await repository.GetAllAsync();
        Assert.Empty(movies);
    }

    [Fact]
    public async Task TryGetByIdAsync_ShouldReturnMovie_WhenMovieExists()
    {
        await using var context = CreateContext();

        var movie = new Movie("Inception", MovieStatus.ToWatch);
        await using var seedContext = CreateContext();
        seedContext.Movies.Add(movie);
        await seedContext.SaveChangesAsync();

        var repository = new MovieRepository(context);
        var foundMovie = await repository.TryGetByIdAsync(movie.Id);
        Assert.NotNull(foundMovie);
        Assert.Equal(movie, foundMovie);
    }

    [Fact]
    public async Task TryGetByIdAsync_ShouldReturnNull_WhenMovieNotFound()
    {
        await using var context = CreateContext();
        var repository = new MovieRepository(context);

        var foundMovie = await repository.TryGetByIdAsync(Guid.NewGuid());
        Assert.Null(foundMovie);
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldDeleteMovie_WhenMovieExists()
    {
        await using var context = CreateContext();

        var movie = new Movie("Inception", MovieStatus.ToWatch);
        await using var seedContext = CreateContext();
        seedContext.Movies.Add(movie);
        await seedContext.SaveChangesAsync();

        var repository = new MovieRepository(context);
        var isDeleted = await repository.DeleteByIdAsync(movie.Id);
        Assert.True(isDeleted);

        await using var assertContext = CreateContext();
        var movies = await assertContext.Movies.ToListAsync();
        Assert.Empty(movies);
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldReturnFalse_WhenMovieNotFound()
    {
        await using var context = CreateContext();
        var repository = new MovieRepository(context);

        var isDeleted = await repository.DeleteByIdAsync(Guid.NewGuid());
        Assert.False(isDeleted);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenMovieNotFound()
    {
        await using var context = CreateContext();
        var repository = new MovieRepository(context);
        var movie = new Movie("Inception", MovieStatus.ToWatch);

        var isUpdated = await repository.UpdateAsync(movie);
        Assert.False(isUpdated);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateMovie_WhenMovieExists()
    {
        await using var context = CreateContext();

        var movie = new Movie("Inception", MovieStatus.ToWatch);
        await using var seedContext = CreateContext();
        seedContext.Movies.Add(movie);
        await seedContext.SaveChangesAsync();

        var repository = new MovieRepository(context);
        movie.Title = "New Title";
        var isUpdated = await repository.UpdateAsync(movie);
        Assert.True(isUpdated);

        await using var assertContext = CreateContext();
        var updatedMovie = await assertContext.Movies.SingleAsync(m => m.Id == movie.Id);

        Assert.Equal(movie, updatedMovie);
    }
}