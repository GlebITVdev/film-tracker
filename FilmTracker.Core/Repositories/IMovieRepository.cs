using System.Collections.Immutable;
using FilmTracker.Core.Models;

namespace FilmTracker.Core.Repositories;

public interface IMovieRepository
{
    Task<ImmutableArray<Movie>> GetAllAsync();
    Task<ImmutableArray<Movie>> GetByStatusAsync(MovieStatus status);
    Task<Movie?> GetByIdAsync(Guid id);
    Task AddAsync(Movie movie);
    Task<bool> DeleteByIdAsync(Guid id);
    Task<bool> UpdateAsync(Movie movie);
}