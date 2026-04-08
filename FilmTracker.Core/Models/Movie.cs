namespace FilmTracker.Core.Models;

public class Movie
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public MovieStatus Status { get; set; }

    public Movie(string title, MovieStatus status)
    {
        Id = Guid.NewGuid();
        Title = title;
        Status = status;
    }   
}