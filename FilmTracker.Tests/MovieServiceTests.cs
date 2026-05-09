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
    }

    [Fact]
    public async Task AddMovieAsync_ShouldReturnTrue_WhenTitleIsValid()
    {
        var mockRepo = new Mock<IMovieRepository>();
        
        mockRepo.Setup(r =>  r.AddAsync(It.IsAny<Movie>())).Returns(Task.CompletedTask);
        
        var service = new MovieService(mockRepo.Object);
        
        var result = await service.AddMovieAsync("Inception", MovieStatus.ToWatch);
        Assert.True(result);
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
    }

    [Fact]
    public async Task EditMovieTitleAsync_ShouldReturnFalse_WhenMovieNotFound()
    {
        var mockRepo = new Mock<IMovieRepository>();

        mockRepo.Setup(r => r.TryGetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Movie?)null);
        
        var service = new MovieService(mockRepo.Object);
        
        var result = await service.EditMovieTitleAsync(Guid.NewGuid(), "New Title");
        
        Assert.False(result);
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
    }
}
