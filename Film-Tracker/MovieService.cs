namespace Film_Tracker;

public class MovieService
{
    private List<Movie> _movies;

    public MovieService()
    {
        _movies = new List<Movie>();
    }

    public void Add()
    {
        Console.WriteLine("Enter movie title:");
        var titleInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(titleInput))
        {
            Console.WriteLine("Title cannot be empty!");
            Pause();
            return;
        }

        Console.WriteLine("Select status:");
        Console.WriteLine("1. To Watch");
        Console.WriteLine("2. Watched");

        int choice = ReadNumber();

        MovieStatus status = choice switch
        {
            1 => MovieStatus.ToWatch,
            2 => MovieStatus.Watched,
            _ => throw new Exception("Invalid status")
        };

        _movies.Add(new Movie(titleInput, status));

        Console.WriteLine("Movie added!");
        Pause();
    }

    public void ShowAll()
    {
        if (_movies.Count == 0)
        {
            Console.WriteLine("No movies yet!");
            Pause();
            return;
        }

        Console.WriteLine("To Watch:");
        PrintMovies(_movies.Where(m => m.Status == MovieStatus.ToWatch).ToList());

        Console.WriteLine("\nWatched:");
        PrintMovies(_movies.Where(m => m.Status == MovieStatus.Watched).ToList());

        Pause();
    }

    public void Delete()
    {
        if (_movies.Count == 0)
        {
            Console.WriteLine("No movies to delete!");
            Pause();
            return;
        }

        Console.WriteLine("Select a movie to delete:");
        PrintMovies(_movies);

        int choice = ReadNumber();

        if (choice < 1 || choice > _movies.Count)
        {
            Console.WriteLine("Invalid number!");
            Pause();
            return;
        }

        var removedMovie = _movies[choice - 1];
        _movies.RemoveAt(choice - 1);

        Console.WriteLine($"Movie \"{removedMovie.Title}\" removed!");
        Pause();
    }

    public void MarkAsWatched()
    {
        var toWatchMovies = _movies
            .Where(m => m.Status == MovieStatus.ToWatch)
            .ToList();

        if (toWatchMovies.Count == 0)
        {
            Console.WriteLine("No movies to mark as watched!");
            Pause();
            return;
        }

        Console.WriteLine("Select a movie to mark as watched:");
        PrintMovies(toWatchMovies);

        int choice = ReadNumber();

        if (choice < 1 || choice > toWatchMovies.Count)
        {
            Console.WriteLine("Invalid number!");
            Pause();
            return;
        }

        var movie = toWatchMovies[choice - 1];
        movie.Status = MovieStatus.Watched;

        Console.WriteLine($"Movie \"{movie.Title}\" marked as watched!");
        Pause();
    }

    private int ReadNumber()
    {
        while (true)
        {
            var input = Console.ReadLine();

            if (int.TryParse(input, out int number))
            {
                return number;
            }

            Console.WriteLine("Invalid input! Please enter a number:");
        }
    }

    private void PrintMovies(List<Movie> movies)
    {
        for (int i = 0; i < movies.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {movies[i].Title}");
        }
    }

    private void Pause()
    {
        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
    }
}