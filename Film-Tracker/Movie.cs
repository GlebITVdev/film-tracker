namespace Film_Tracker;

public enum MovieStatus
{
    ToWatch,
    Watched
}
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