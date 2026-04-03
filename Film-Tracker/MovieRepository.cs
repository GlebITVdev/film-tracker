namespace Film_Tracker;

public class MovieRepository
{
    private readonly List<Movie> _movies = [];

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