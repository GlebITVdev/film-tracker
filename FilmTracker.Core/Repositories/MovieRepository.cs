using FilmTracker.Core.Models;

namespace FilmTracker.Core.Repositories;

public class MovieRepository
{
    private readonly List<Movie> _movies = new();

    public void Add(Movie movie)
    {
        _movies.Add(movie);
    }
    public List<Movie> GetAll()
    {
        return _movies.ToList();
    }
    public List<Movie> GetByStatus(MovieStatus status)
    {
        return _movies
            .Where(m => m.Status == status)
            .ToList();
    }
    public Movie? GetById(Guid id)
    {
        return _movies.FirstOrDefault(m => m.Id == id);
    }
    public bool DeleteById(Guid id)
    {
        var movie = _movies.FirstOrDefault(m => m.Id == id);

        if (movie == null)
        {
            return false;
        }

        _movies.Remove(movie);
        return true;
    }
    public bool Update(Movie updatedMovie)
    {
        var movie = _movies.FirstOrDefault(m => m.Id == updatedMovie.Id);

        if (movie == null)
        {
            return false;
        }

        movie.Title = updatedMovie.Title;
        movie.Status = updatedMovie.Status;

        return true;
    }
}