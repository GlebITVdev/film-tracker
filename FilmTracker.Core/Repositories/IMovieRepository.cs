using FilmTracker.Core.Models;

namespace FilmTracker.Core.Repositories;

public interface IMovieRepository
{
    void Add(Movie movie);
    List<Movie> GetAll();
    List<Movie> GetByStatus(MovieStatus status);
    Movie? GetById(Guid id);
    bool DeleteById(Guid id);
    bool Update(Movie movie);
}