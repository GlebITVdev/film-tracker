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

    public async Task InitializeAsync() =>
        await _databaseFixture.ClearDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task AddAsync_ShouldSaveCorrectData()
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

        var inception = new Movie(Guid.NewGuid(), "Inception", MovieStatus.ToWatch);
        var interstellar = new Movie(Guid.NewGuid(), "Interstellar", MovieStatus.ToWatch);
        var darkKnight = new Movie(Guid.NewGuid(), "Dark Knight", MovieStatus.Watched);

        seedContext.Movies.AddRange(interstellar, darkKnight, inception);

        await seedContext.SaveChangesAsync();

        var repository = new MovieRepository(context);
        var movies = await repository.GetByStatusAsync(MovieStatus.ToWatch);

        var expected = new[]
        {
            inception,
            interstellar,
        };

        Assert.Equivalent(expected, movies);
    }

    [Fact]
    public async Task GetByStatusAsync_ShouldReturnOnlyWatchedMovies_WhenStatusIsWatched()
    {
        await using var context = CreateContext();
        await using var seedContext = CreateContext();

        var inception = new Movie(Guid.NewGuid(), "Inception", MovieStatus.ToWatch);
        var interstellar = new Movie(Guid.NewGuid(), "Interstellar", MovieStatus.Watched);
        var darkKnight = new Movie(Guid.NewGuid(), "Dark Knight", MovieStatus.Watched);

        seedContext.Movies.AddRange(interstellar, darkKnight, inception);
        await seedContext.SaveChangesAsync();

        var repository = new MovieRepository(context);
        var movies = await repository.GetByStatusAsync(MovieStatus.Watched);

        var expected = new[]
        {
            interstellar,
            darkKnight,
        };

        Assert.Equivalent(expected, movies);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllMovies()
    {
        await using var context = CreateContext();
        await using var seedContext = CreateContext();

        var inception = new Movie(Guid.NewGuid(), "Inception", MovieStatus.ToWatch);
        var darkKnight = new Movie(Guid.NewGuid(), "Dark Knight", MovieStatus.Watched);

        seedContext.Movies.AddRange(inception, darkKnight);

        await seedContext.SaveChangesAsync();

        var repository = new MovieRepository(context);
        var movies = await repository.GetAllAsync();

        var expected = new[]
        {
            inception,
            darkKnight
        };

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

        var existingMovie = new Movie(Guid.NewGuid(), "Inception", MovieStatus.ToWatch);
        await using var seedContext = CreateContext();
        seedContext.Movies.Add(existingMovie);
        await seedContext.SaveChangesAsync();

        var repository = new MovieRepository(context);
        var nonExistingMovieId = Guid.NewGuid();

        var isDeleted = await repository.DeleteByIdAsync(nonExistingMovieId);

        Assert.False(isDeleted);

        await using var assertContext = CreateContext();
        var movieInDatabase = await assertContext.Movies.SingleAsync();

        Assert.Equal(existingMovie, movieInDatabase);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenMovieNotFound()
    {
        await using var context = CreateContext();

        var existingMovie = new Movie(Guid.NewGuid(), "Interstellar", MovieStatus.Watched);

        await using var seedContext = CreateContext();
        seedContext.Movies.Add(existingMovie);
        await seedContext.SaveChangesAsync();

        var nonExistingMovie = new Movie(Guid.NewGuid(), "Inception", MovieStatus.ToWatch);

        var repository = new MovieRepository(context);
        var isUpdated = await repository.UpdateAsync(nonExistingMovie);

        Assert.False(isUpdated);

        await using var assertContext = CreateContext();
        var movieInDatabase = await assertContext.Movies.SingleAsync();

        Assert.Equal(existingMovie, movieInDatabase);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateMovie_WhenMovieExists()
    {
        await using var context = CreateContext();

        var movieToUpdate = new Movie(Guid.NewGuid(), "Inception", MovieStatus.ToWatch);
        var otherMovie = new Movie(Guid.NewGuid(), "Interstellar", MovieStatus.Watched);

        await using var seedContext = CreateContext();
        seedContext.Movies.AddRange(movieToUpdate, otherMovie);
        await seedContext.SaveChangesAsync();

        var repository = new MovieRepository(context);
        movieToUpdate.Title = "New Title";
        var isUpdated = await repository.UpdateAsync(movieToUpdate);
        Assert.True(isUpdated);

        await using var assertContext = CreateContext();

        var updatedMovie = await assertContext.Movies.SingleAsync(m => m.Id == movieToUpdate.Id);
        var unchangedMovie = await assertContext.Movies.SingleAsync(m => m.Id == otherMovie.Id);

        Assert.Equal(movieToUpdate, updatedMovie);
        Assert.Equal(otherMovie, unchangedMovie);
    }
}