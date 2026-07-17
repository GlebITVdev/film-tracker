using FilmTracker.Core.Data;
using FilmTracker.Core.Models;
using FilmTracker.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FilmTracker.Tests;

public class MovieRepositoryTests : IAsyncLifetime
{
    private const string ConnectionString =
        "Host=localhost;Port=5432;Database=filmtracker_test_db;Username=postgres;Password=postgres";
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new AppDbContext(options);
    }
    
    public async Task InitializeAsync()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }
    
    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<AppDbContext> SetupAsync()
    {
        var context = CreateContext();
        await context.Movies.ExecuteDeleteAsync();
        return context;
    }
    
    [Fact]
    public async Task AddAsync_ShouldAddMovie_WhenMovieIsValid()
    {
        await using var context = await SetupAsync();
        var repository = new MovieRepository(context);
        var movie = new Movie("Inception", MovieStatus.ToWatch);
        
        await repository.AddAsync(movie);
        
        await using var assertContext = CreateContext();
        var movies = await assertContext.Movies.ToListAsync();
        Assert.Single(movies);
    }
    
    [Fact]
    public async Task AddAsync_ShouldSaveCorrectData_WhenMovieIsAdded()
    {
        await using var context = await SetupAsync();
        var repository = new MovieRepository(context);
        var movie = new Movie("Inception", MovieStatus.ToWatch);
        
        await repository.AddAsync(movie);

        await using var assertContext = CreateContext();
        var addedMovie = await assertContext.Movies.SingleAsync();
        Assert.Equal("Inception", addedMovie.Title);
        Assert.Equal(MovieStatus.ToWatch, addedMovie.Status);
    }

    [Fact]
    public async Task GetByStatusAsync_ShouldReturnOnlyToWatchMovies_WhenStatusIsToWatch()
    {
        await using var context = await SetupAsync();
        await using var seedContext = CreateContext();
        seedContext.Movies.AddRange(
            new Movie("Inception", MovieStatus.ToWatch),
            new Movie("Interstellar", MovieStatus.ToWatch),
            new Movie("The Dark Knight", MovieStatus.Watched));
        await seedContext.SaveChangesAsync();

        var repository = new MovieRepository(context);
        var movies = await repository.GetByStatusAsync(MovieStatus.ToWatch);
        Assert.Equal(2, movies.Length);
        Assert.All(movies, m => Assert.Equal(MovieStatus.ToWatch, m.Status));
    }
    
    [Fact]
    public async Task GetByStatusAsync_ShouldReturnOnlyWatchedMovies_WhenStatusIsWatched()
    {
        await using var context = await SetupAsync();

        await using var seedContext = CreateContext();
        seedContext.Movies.AddRange(
            new Movie("Inception", MovieStatus.ToWatch),
            new Movie("Interstellar", MovieStatus.Watched),
            new Movie("The Dark Knight", MovieStatus.Watched));
        await seedContext.SaveChangesAsync();

        var repository = new MovieRepository(context);
        var movies = await repository.GetByStatusAsync(MovieStatus.Watched);
        Assert.Equal(2, movies.Length);
        Assert.All(movies, m => Assert.Equal(MovieStatus.Watched, m.Status));
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllMovies_WhenMoviesExist()
    {
        await using var context = await SetupAsync();

        await using var seedContext = CreateContext();
        seedContext.Movies.AddRange(
            new Movie("Inception", MovieStatus.ToWatch),
            new Movie("The Dark Knight", MovieStatus.Watched));
        await seedContext.SaveChangesAsync();

        var repository = new MovieRepository(context);
        var movies = await repository.GetAllAsync();
        Assert.Equal(2, movies.Length);
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenNoMoviesExist()
    {
        await using var context = await SetupAsync();
        var repository = new MovieRepository(context);

        var movies = await repository.GetAllAsync();
        Assert.Empty(movies);
    }

    [Fact]
    public async Task TryGetByIdAsync_ShouldReturnMovie_WhenMovieExists()
    {
        await using var context = await SetupAsync();
        
        var movie = new Movie("Inception", MovieStatus.ToWatch);
        await using var seedContext = CreateContext();
        seedContext.Movies.Add(movie);
        await seedContext.SaveChangesAsync();
        
        var repository = new MovieRepository(context);
        var foundMovie = await repository.TryGetByIdAsync(movie.Id);
        Assert.NotNull(foundMovie);
        Assert.Equal(movie.Id, foundMovie.Id);
    }
    
    [Fact]
    public async Task TryGetByIdAsync_ShouldReturnNull_WhenMovieNotFound()
    {
        await using var context = await SetupAsync();
        var repository = new MovieRepository(context);

        var foundMovie = await repository.TryGetByIdAsync(Guid.NewGuid());
        Assert.Null(foundMovie);
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldDeleteMovie_WhenMovieExists()
    {
        await using var context = await SetupAsync();

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
        await using var context = await SetupAsync();
        var repository = new MovieRepository(context);

        var isDeleted = await repository.DeleteByIdAsync(Guid.NewGuid());
        Assert.False(isDeleted);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenMovieNotFound()
    {
        await using var context = await SetupAsync();
        var repository = new MovieRepository(context);
        var movie = new Movie("Inception", MovieStatus.ToWatch);

        var isUpdated = await repository.UpdateAsync(movie);
        Assert.False(isUpdated);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldUpdateMovie_WhenMovieExists()
    {
        await using var context = await SetupAsync();

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
        Assert.Equal("New Title", updatedMovie.Title);
    }
}