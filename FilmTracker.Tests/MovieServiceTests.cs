using System.Collections.Immutable;
using FilmTracker.Core.Models;
using FilmTracker.Core.Repositories;
using FilmTracker.Core.Services;
using Moq;

namespace FilmTracker.Tests;

public class MovieServiceTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task AddMovieAsync_ShouldReturnFalse_WhenTitleIsEmpty(string? title)
    {
        var mockRepo = new Mock<IMovieRepository>();
        var service = new MovieService(mockRepo.Object);

        var result = await service.AddMovieAsync(title, MovieStatus.ToWatch);
        
        Assert.False(result);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<Movie>()), Times.Never);
    }

    [Fact]
    public async Task AddMovieAsync_ShouldReturnTrue_WhenTitleIsValid()
    {
        var mockRepo = new Mock<IMovieRepository>();
    
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Movie>())).Returns(Task.CompletedTask);
    
        var service = new MovieService(mockRepo.Object);
    
        var result = await service.AddMovieAsync("Inception", MovieStatus.ToWatch);
    
        Assert.True(result);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<Movie>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsWatchedAsync_ShouldReturnFalse_WhenMovieNotFound()
    {
        var mockRepo = new Mock<IMovieRepository>();
        
        mockRepo.Setup(r => r.TryGetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Movie?)null);
        
        var service = new MovieService(mockRepo.Object);
        
        var result = await service.MarkAsWatchedAsync(Guid.NewGuid());
        
        Assert.False(result);
        mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Movie>()), Times.Never);
    }

    [Fact]
    public async Task MarkAsWatchedAsync_ShouldReturnTrue_AndChangeStatus()
    {
        var mockRepo = new Mock<IMovieRepository>();
        var movie = new Movie("Inception", MovieStatus.ToWatch);
        
        mockRepo.Setup(r => r.TryGetByIdAsync(movie.Id))
            .ReturnsAsync(movie);
        
        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Movie>()))
            .ReturnsAsync(true);
        
        var service = new MovieService(mockRepo.Object);
        
        var result = await service.MarkAsWatchedAsync(movie.Id);
        
        Assert.True(result);
        Assert.Equal(MovieStatus.Watched, movie.Status);
        mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Movie>()), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task EditMovieTitleAsync_ShouldReturnFalse_WhenTitleIsEmpty(string? title)
    {
        var mockRepo = new Mock<IMovieRepository>();
        var service = new MovieService(mockRepo.Object);
        
        var result = await service.EditMovieTitleAsync(Guid.NewGuid(), title);
        
        Assert.False(result);
        mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Movie>()), Times.Never);
    }

    [Fact]
    public async Task EditMovieTitleAsync_ShouldReturnFalse_WhenMovieNotFound()
    {
        var mockRepo = new Mock<IMovieRepository>();

        mockRepo.Setup(r => r.TryGetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Movie?)null);
        
        var service = new MovieService(mockRepo.Object);
        
        var result = await service.EditMovieTitleAsync(Guid.NewGuid(), "New Title");
        
        Assert.False(result);
        mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Movie>()), Times.Never);
    }
    
    [Fact]
    public async Task EditMovieTitleAsync_ShouldReturnTrue_AndChangeTitle()
    {
        var mockRepo = new Mock<IMovieRepository>();
        var movie = new Movie("Inception", MovieStatus.ToWatch);

        mockRepo.Setup(r => r.TryGetByIdAsync(movie.Id))
            .ReturnsAsync(movie);

        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Movie>()))
            .ReturnsAsync(true);

        var service = new MovieService(mockRepo.Object);

        var result = await service.EditMovieTitleAsync(movie.Id, "New Title");

        Assert.True(result);
        Assert.Equal("New Title", movie.Title);
        mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Movie>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteMovieAsync_ShouldReturnFalse_WhenMovieNotFound()
    {
        var mockRepo = new Mock<IMovieRepository>();

        mockRepo.Setup(r => r.DeleteByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(false);
        
        var service = new MovieService(mockRepo.Object);
        
        var result = await service.DeleteMovieAsync(Guid.NewGuid());
        
        Assert.False(result);
        mockRepo.Verify(r => r.DeleteByIdAsync(It.IsAny<Guid>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteMovieAsync_ShouldReturnTrue_WhenMovieIsDeleted()
    {
        var mockRepo = new Mock<IMovieRepository>();

        mockRepo.Setup(r => r.DeleteByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        
        var service = new MovieService(mockRepo.Object);
        
        var result = await service.DeleteMovieAsync(Guid.NewGuid());
        
        Assert.True(result);
        mockRepo.Verify(r => r.DeleteByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetAllMoviesAsync_ShouldReturnEmptyArray_WhenNoMoviesExist()
    {
        var mockRepo = new Mock<IMovieRepository>();
        mockRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(ImmutableArray<Movie>.Empty);
        
        var service = new MovieService(mockRepo.Object);
        
        var result = await service.GetAllMoviesAsync();
        Assert.Empty(result);
        mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllMoviesAsync_ShouldReturnAllMovies_WhenMoviesExist()
    {
        var mockRepo = new Mock<IMovieRepository>();
        var movies = new[]
        {
            new Movie("Inception", MovieStatus.ToWatch),
            new Movie("Interstellar", MovieStatus.Watched)
        }.ToImmutableArray();

        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(movies);
        
        var service = new MovieService(mockRepo.Object);
        
        var result = await service.GetAllMoviesAsync();
        Assert.Equal(movies, result);
        mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }
    
    [Fact]
    public async Task GetToWatchMoviesAsync_ShouldReturnEmptyArray_WhenNoMoviesExist()
    {
        var mockRepo = new Mock<IMovieRepository>();
        mockRepo.Setup(r => r.GetByStatusAsync(MovieStatus.ToWatch))
            .ReturnsAsync(ImmutableArray<Movie>.Empty);
        
        var service = new MovieService(mockRepo.Object);
        
        var result = await service.GetToWatchMoviesAsync();
        Assert.Empty(result);
        mockRepo.Verify(r => r.GetByStatusAsync(MovieStatus.ToWatch), Times.Once);
    }

    [Fact]
    public async Task GetToWatchMoviesAsync_ShouldReturnToWatchMovies_WhenMoviesExist()
    {
        var mockRepo = new Mock<IMovieRepository>();
        var movies = new[]
        {
            new Movie("Inception", MovieStatus.ToWatch),
            new Movie("Interstellar", MovieStatus.ToWatch),
        }.ToImmutableArray();

        mockRepo.Setup(r => r.GetByStatusAsync(MovieStatus.ToWatch)).ReturnsAsync(movies);
        
        var service = new MovieService(mockRepo.Object);
        
        var result = await service.GetToWatchMoviesAsync();
        Assert.Equal(movies, result);
        mockRepo.Verify(r => r.GetByStatusAsync(MovieStatus.ToWatch), Times.Once);
    }

    [Fact]
    public async Task GetWatchedMoviesAsync_ShouldReturnEmptyArray_WhenNoMoviesExist()
    {
        var mockRepo = new Mock<IMovieRepository>();
        
        mockRepo.Setup(r => r.GetByStatusAsync(MovieStatus.Watched))
            .ReturnsAsync(ImmutableArray<Movie>.Empty);
        
        var service = new MovieService(mockRepo.Object);
        var result = await service.GetWatchedMoviesAsync();
        Assert.Empty(result);
        mockRepo.Verify(r => r.GetByStatusAsync(MovieStatus.Watched), Times.Once);
    }

    [Fact]
    public async Task GetWatchedMoviesAsync_ShouldReturnWatchedMovies_WhenMoviesExist()
    {
        var mockRepo = new Mock<IMovieRepository>();
        var movies = new[]
        {
            new Movie("Inception", MovieStatus.Watched),
            new Movie("Interstellar", MovieStatus.Watched),
        }.ToImmutableArray();

        mockRepo.Setup(r => r.GetByStatusAsync(MovieStatus.Watched)).ReturnsAsync(movies);
        
        var service = new MovieService(mockRepo.Object);
        
        var result = await service.GetWatchedMoviesAsync();
        Assert.Equal(movies, result);
        mockRepo.Verify(r => r.GetByStatusAsync(MovieStatus.Watched), Times.Once);
    }
}
