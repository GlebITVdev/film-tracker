namespace FilmTracker.Core.Models;

public record Movie
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public MovieStatus Status { get; set; }

    public Movie(Guid id, string title, MovieStatus status)
    {
        Id = id;
        Title = title;
        Status = status;
    }
}