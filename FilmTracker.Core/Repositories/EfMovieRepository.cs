using FilmTracker.Core.Data;
using FilmTracker.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FilmTracker.Core.Repositories;

public class EfMovieRepository : IMovieRepository
{
    private readonly AppDbContext _context;

    public EfMovieRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Movie movie)
    {
        _context.Movies.Add(movie);
        _context.SaveChanges();
    }

    public List<Movie> GetAll()
    {
        return _context.Movies.ToList();
    }

    public List<Movie> GetByStatus(MovieStatus status)
    {
        return _context.Movies
            .Where(m => m.Status == status)
            .ToList();
    }

    public Movie? GetById(Guid id)
    {
        return _context.Movies.FirstOrDefault(m => m.Id == id);
    }

    public bool DeleteById(Guid id)
    {
        var movie = _context.Movies.FirstOrDefault(m => m.Id == id);

        if (movie == null)
        {
            return false;
        }

        _context.Movies.Remove(movie);
        _context.SaveChanges();
        return true;
    }

    public bool Update(Movie updatedMovie)
    {
        var existingMovie = _context.Movies.FirstOrDefault(m => m.Id == updatedMovie.Id);

        if (existingMovie == null)
        {
            return false;
        }

        existingMovie.Title = updatedMovie.Title;
        existingMovie.Status = updatedMovie.Status;

        _context.SaveChanges();
        return true;
    }
}