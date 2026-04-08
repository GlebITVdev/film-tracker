using FilmTracker.Core.Models;
using FilmTracker.Core.Repositories;

namespace FilmTracker.Core.Services;

public class MovieService
{
    private readonly MovieRepository _repository;
    public MovieService()
    {
        _repository = new MovieRepository();
    }
    public bool AddMovie(string title, MovieStatus status)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return false;
        }

        var movie = new Movie(title, status);
        _repository.Add(movie);
        return true;
    }
    public List<Movie> GetAllMovies()
    {
        return _repository.GetAll();
    }
    public List<Movie> GetToWatchMovies()
    {
        return _repository.GetByStatus(MovieStatus.ToWatch);
    }
    public List<Movie> GetWatchedMovies()
    {
        return _repository.GetByStatus(MovieStatus.Watched);
    }
    public bool DeleteMovie(Guid id)
    {
        return _repository.DeleteById(id);
    }
    public bool MarkAsWatched(Guid id)
    {
        return _repository.UpdateStatus(id, MovieStatus.Watched);
    }
}