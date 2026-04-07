namespace FilmTracker.Core.Models;

public class Movie
{
    public string Title { get; set; }
    public MovieStatus Status { get; set; }

    public Movie(string title, MovieStatus status)
    {
        Title = title;
        Status = status;
    }
}