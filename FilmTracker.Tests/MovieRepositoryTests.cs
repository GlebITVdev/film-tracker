using FilmTracker.Core.Data;
using FilmTracker.Core.Models;
using FilmTracker.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using FilmTracker.Tests.Infrastructure;

namespace FilmTracker.Tests;

public class MovieRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private readonly DatabaseFixture _databaseFixture;

    public MovieRepositoryTests(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
    }

    private AppDbContext CreateContext() => _databaseFixture.CreateContext();

    private async Task ClearDatabaseAsync()
    {
        await using var context = CreateContext();
        await context.Movies.ExecuteDeleteAsync();
    }

    public async Task InitializeAsync() => await ClearDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task AddAsync_ShouldSaveCorrectData_WhenMovieIsAdded()
    {
        await using var context = CreateContext();
        var repository = new MovieRepository(context);

        var movieId = Guid.NewGuid();
        var movie = new Movie(movieId, "Inception", MovieStatus.ToWatch);

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
            new Movie(Guid.NewGuid(), "Inception", MovieStatus.ToWatch),
            new Movie(Guid.NewGuid(), "Interstellar", MovieStatus.ToWatch)
        };

        seedContext.Movies.AddRange(expected);
        seedContext.Movies.Add(new Movie(Guid.NewGuid(), "The Dark Knight", MovieStatus.Watched));
        await seedContext.SaveChangesAsync();

        var repository = new MovieRepository(context);
        var movies = await repository.GetByStatusAsync(MovieStatus.ToWatch);

        Assert.Equivalent(expected, movies);
    }

    [Fact]
    public async Task GetByStatusAsync_ShouldReturnOnlyWatchedMovies_WhenStatusIsWatched()
    {
        await using var context = CreateContext();
        await using var seedContext = CreateContext();

        var expected = new[]
        {
            new Movie(Guid.NewGuid(), "Interstellar", MovieStatus.Watched),
            new Movie(Guid.NewGuid(), "The Dark Knight", MovieStatus.Watched)
        };

        seedContext.Movies.Add(new Movie(Guid.NewGuid(), "Inception", MovieStatus.ToWatch));
        seedContext.Movies.AddRange(expected);
        await seedContext.SaveChangesAsync();

        var repository = new MovieRepository(context);
        var movies = await repository.GetByStatusAsync(MovieStatus.Watched);

        Assert.Equivalent(expected, movies);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllMovies()
    {
        await using var context = CreateContext();
        await using var seedContext = CreateContext();

        var expected = new[]
        {
            new Movie(Guid.NewGuid(), "Inception", MovieStatus.ToWatch),
            new Movie(Guid.NewGuid(), "The Dark Knight", MovieStatus.Watched)
        };

        seedContext.Movies.AddRange(expected);
        await seedContext.SaveChangesAsync();

        var repository = new MovieRepository(context);
        var movies = await repository.GetAllAsync();

        Assert.Equivalent(expected, movies);
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

        var movie = new Movie(Guid.NewGuid(), "Inception", MovieStatus.ToWatch);
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

        var movieId = Guid.NewGuid();
        var movie = new Movie(movieId, "Inception", MovieStatus.ToWatch);
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

        var movieId = Guid.NewGuid();
        var movie = new Movie(movieId, "Inception", MovieStatus.ToWatch);

        var isUpdated = await repository.UpdateAsync(movie);
        Assert.False(isUpdated);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateMovie_WhenMovieExists()
    {
        await using var context = CreateContext();

        var movieId = Guid.NewGuid();
        var movie = new Movie(movieId, "Inception", MovieStatus.ToWatch);
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