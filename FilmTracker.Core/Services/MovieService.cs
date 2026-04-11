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
    public bool AddMovie(string title, MovieStatus status)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return false;
        }

        var movie = new Movie(title.Trim(), status);
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
        var movie = _repository.GetById(id);

        if (movie == null)
        {
            return false;
        }
        movie.Status = MovieStatus.Watched;
        return _repository.Update(movie);
    }
}