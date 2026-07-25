namespace FilmTracker.Core.Models;

public record Movie(Guid Id, string Title, MovieStatus Status)
{
    public Guid Id { get; set; } = Id;
    public string Title { get; set; } = Title;
    public MovieStatus Status { get; set; } = Status;
}