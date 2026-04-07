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
        return _repository
            .GetAll()
            .Where(m => m.Status == MovieStatus.ToWatch)
            .ToList();
    }

    public List<Movie> GetWatchedMovies()
    {
        return _repository
            .GetAll()
            .Where(m => m.Status == MovieStatus.Watched)
            .ToList();
    }

    public bool DeleteMovie(int index)
    {
        var movies = _repository.GetAll();

        if (index < 0 || index >= movies.Count)
        {
            return false;
        }

        _repository.RemoveAt(index);
        return true;
    }

    public bool MarkAsWatched(int index)
    {
        var toWatchMovies = GetToWatchMovies();

        if (index < 0 || index >= toWatchMovies.Count)
        {
            return false;
        }

        toWatchMovies[index].Status = MovieStatus.Watched;
        return true;
    }
}