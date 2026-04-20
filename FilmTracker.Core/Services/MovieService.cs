using System.Collections.Immutable;
using FilmTracker.Core.Models;
using FilmTracker.Core.Repositories;

namespace FilmTracker.Core.Services;

public class MovieService
{
    private readonly IMovieRepository _repository;
    public MovieService(IMovieRepository repository)
    {
        _repository = repository;
    }
    public async Task<bool> AddMovieAsync(string title, MovieStatus status)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return false;
        }

        var movie = new Movie(title.Trim(), status);
        await _repository.AddAsync(movie);
        return true;
    }
    public async Task<ImmutableArray<Movie>> GetAllMoviesAsync()
    {
        return await _repository.GetAllAsync();
    }
    public async  Task<ImmutableArray<Movie>> GetToWatchMoviesAsync()
    {
        return await _repository.GetByStatusAsync(MovieStatus.ToWatch);
    }
    public async Task<ImmutableArray<Movie>> GetWatchedMoviesAsync()
    {
        return await _repository.GetByStatusAsync(MovieStatus.Watched);
    }
    public async Task<bool> DeleteMovieAsync(Guid id)
    {
        return await _repository.DeleteByIdAsync(id);
    }
    public async Task<bool> MarkAsWatchedAsync(Guid id)
    {
        var movie = await _repository.TryGetByIdAsync(id);

        if (movie == null)
        {
            return false;
        }
        movie.Status = MovieStatus.Watched;
        return await _repository.UpdateAsync(movie);
    }

    public async Task<bool> EditMovieTitleAsync(Guid id, string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
        {
            return false; 
        }
        var movie = await _repository.TryGetByIdAsync(id);
        if (movie == null)
        {
            return false;
        }
        movie.Title = newTitle;
        
        return await _repository.UpdateAsync(movie);
    }
}