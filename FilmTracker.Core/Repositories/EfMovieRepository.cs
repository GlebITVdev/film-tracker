using FilmTracker.Core.Data;
using FilmTracker.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;


namespace FilmTracker.Core.Repositories;

public class EfMovieRepository : IMovieRepository
{
    private readonly AppDbContext _context;

    public EfMovieRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Movie movie)
    {
        await _context.Movies.AddAsync(movie);
        await _context.SaveChangesAsync();
    }

    public async Task<ImmutableArray<Movie>> GetAllAsync()
    {
        var list = await _context.Movies.ToListAsync();
        return list.ToImmutableArray();
    }

    public async Task<ImmutableArray<Movie>> GetByStatusAsync(MovieStatus status)
    {
        var list = await _context.Movies
            .Where(m => m.Status == status)
            .ToListAsync();
        return list.ToImmutableArray();
    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        return await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        var affectedRows = await _context.Movies
            .Where(m => m.Id == id)
            .ExecuteDeleteAsync();

        return affectedRows > 0;
    }

    public async Task<bool> UpdateAsync(Movie updatedMovie)
    {
        var affectedRows = await _context.Movies
            .Where(m => m.Id == updatedMovie.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.Title, updatedMovie.Title)
                .SetProperty(m => m.Status, updatedMovie.Status));

        return affectedRows > 0;
    }
}