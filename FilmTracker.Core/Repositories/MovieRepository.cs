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
        return _movies;
    }

    public void RemoveAt(int index)
    {
        _movies.RemoveAt(index);
    }
}