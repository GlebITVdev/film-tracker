using FilmTracker.Core.Data;
using FilmTracker.Core.Models;
using FilmTracker.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace FilmTracker.Tests;

public class MovieRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public MovieRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_fixture.Container.GetConnectionString())
            .Options;

        var context = new AppDbContext(options);
    
        context.Database.EnsureCreated();
        context.Movies.RemoveRange(context.Movies);
        context.SaveChanges();

        return context;
    }

    [Fact]
    public async Task AddAsync_ShouldAddMovie_WhenMovieIsValid()
    {
        await using var context = CreateContext();
        var repository = new MovieRepository(context);
        
        var movie = new Movie("Inception", MovieStatus.ToWatch);
        await repository.AddAsync(movie);
        
        var movies = await repository.GetAllAsync();
        Assert.Single(movies);
    }
    
    [Fact]
    public async Task AddAsync_ShouldSaveCorrectData_WhenMovieIsAdded()
    {
        await using var context = CreateContext();
        var repository = new MovieRepository(context);
        
        var movie = new Movie("Inception", MovieStatus.ToWatch);
        await repository.AddAsync(movie);
        
        var movies = await repository.GetAllAsync();
        var addedMovie = movies.First();
        Assert.Equal("Inception", addedMovie.Title);
        Assert.Equal(MovieStatus.ToWatch, addedMovie.Status);
    }

    [Fact]
    public async Task GetByStatusAsync_ShouldReturnOnlyToWatchMovies_WhenStatusIsToWatch()
    {
        await using var context = CreateContext();
        var repository = new MovieRepository(context);
        
        await repository.AddAsync(new Movie("Inception", MovieStatus.ToWatch));
        await repository.AddAsync(new Movie("Interstellar", MovieStatus.ToWatch));
        await repository.AddAsync(new Movie("The Dark Knight", MovieStatus.Watched));
        
        var movies = await repository.GetByStatusAsync(MovieStatus.ToWatch);
        Assert.Equal(2, movies.Length);
    }
    
    [Fact]
    public async Task GetByStatusAsync_ShouldReturnOnlyWatchedMovies_WhenStatusIsWatched()
    {
        await using var context = CreateContext();
        var repository = new MovieRepository(context);
        
        await repository.AddAsync(new Movie("Inception", MovieStatus.ToWatch));
        await repository.AddAsync(new Movie("Interstellar", MovieStatus.Watched));
        await repository.AddAsync(new Movie("The Dark Knight", MovieStatus.Watched));
        
        var movies = await repository.GetByStatusAsync(MovieStatus.Watched);
        Assert.Equal(2, movies.Length);
    }

    [Fact]
    public async Task TryGetByIdAsync_ShouldReturnMovie_WhenMovieExists()
    {
        await using var context = CreateContext();
        var repository = new MovieRepository(context);
        
        var movie = new Movie("Inception", MovieStatus.ToWatch);
        await repository.AddAsync(movie);
        
        var foundMovie = await repository.TryGetByIdAsync(movie.Id);
        Assert.NotNull(foundMovie);
        Assert.Equal(movie.Id, foundMovie.Id);
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
        var repository = new MovieRepository(context);
        
        var movie = new Movie("Inception", MovieStatus.ToWatch);
        await repository.AddAsync(movie);
        
        var isDeleted = await repository.DeleteByIdAsync(movie.Id);
        Assert.True(isDeleted);

        var movies = await repository.GetAllAsync();
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
        var repository = new MovieRepository(context);

        var movie = new Movie("Inception", MovieStatus.ToWatch);
        await repository.AddAsync(movie);
        
        movie.Title = "New Title";
        var isUpdated = await repository.UpdateAsync(movie);
        Assert.True(isUpdated);
        
        var updatedMovie = await repository.TryGetByIdAsync(movie.Id);
        Assert.Equal("New Title", updatedMovie?.Title);
    }
}