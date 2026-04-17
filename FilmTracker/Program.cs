using FilmTracker.Core.Data;
using FilmTracker.Core.Models;
using FilmTracker.Core.Repositories;
using FilmTracker.Core.Services;

namespace FilmTracker.ConsoleApp;

class Program
{
    static async Task Main(string[] args)
    {
        await using var context = new AppDbContext();

        var repository = new EfMovieRepository(context);
        var service = new MovieService(repository);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to Film Tracker!");
            Console.WriteLine("1. Add movie");
            Console.WriteLine("2. Delete movie");
            Console.WriteLine("3. Show movies");
            Console.WriteLine("4. Mark as watched");
            Console.WriteLine("5. Edit movie title");
            Console.WriteLine("6. Exit");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await AddMovie(service);
                    break;
                case "2":
                    await DeleteMovie(service);
                    break;
                case "3":
                    await ShowMovies(service);
                    break;
                case "4":
                    await MarkAsWatched(service);
                    break;
                case "5":
                    await EditMovieTitle(service);
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("Invalid option!");
                    Pause();
                    break;
            }
        }
    }

    static async Task AddMovie(MovieService service)
    {
        Console.WriteLine("Enter movie title:");
        var title = Console.ReadLine();

        Console.WriteLine("Select status:");
        Console.WriteLine("1. To Watch");
        Console.WriteLine("2. Watched");

        var input = ReadNumber();
        MovieStatus status;

        switch (input)
        {
            case 1:
                status = MovieStatus.ToWatch;
                break;
            case 2:
                status = MovieStatus.Watched;
                break;
            default:
                Console.WriteLine("Invalid status!");
                Pause();
                return;
        }

        var isAdded = await service.AddMovieAsync(title ?? string.Empty, status);

        if (!isAdded)
        {
            Console.WriteLine("Title cannot be empty!");
            Pause();
            return;
        }

        Console.WriteLine("Movie added!");
        Pause();
    }

    static async Task ShowMovies(MovieService service)
    {
        var toWatch = await service.GetToWatchMoviesAsync();
        var watched = await service.GetWatchedMoviesAsync();

        Console.WriteLine("To Watch:");
        if (toWatch.Length == 0)
        {
            Console.WriteLine("No movies to watch yet.");
        }
        else
        {
            foreach (var movie in toWatch)
            {
                Console.WriteLine($"- {movie.Title}");
            }
        }

        Console.WriteLine("\nWatched:");
        if (watched.Length == 0)
        {
            Console.WriteLine("No watched movies yet.");
        }
        else
        {
            foreach (var movie in watched)
            {
                Console.WriteLine($"- {movie.Title}");
            }
        }

        Pause();
    }

    static async Task DeleteMovie(MovieService service)
    {
        var movies = await service.GetAllMoviesAsync();

        if (movies.Length == 0)
        {
            Console.WriteLine("No movies to delete!");
            Pause();
            return;
        }

        Console.WriteLine("Select a movie to delete:");

        for (var i = 0; i < movies.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {movies[i].Title}");
        }

        var choice = ReadNumber();

        if (choice < 1 || choice > movies.Length)
        {
            Console.WriteLine("Invalid number!");
            Pause();
            return;
        }

        var selectedMovie = movies[choice - 1];
        var isDeleted = await service.DeleteMovieAsync(selectedMovie.Id);

        if (!isDeleted)
        {
            Console.WriteLine("Movie was not found!");
            Pause();
            return;
        }

        Console.WriteLine("Movie deleted!");
        Pause();
    }
    
    static async Task MarkAsWatched(MovieService service)
    {
        var toWatchMovies = await service.GetToWatchMoviesAsync();

        if (toWatchMovies.Length == 0)
        {
            Console.WriteLine("No movies to mark as watched!");
            Pause();
            return;
        }

        Console.WriteLine("Select a movie to mark as watched:");

        for (int i = 0; i < toWatchMovies.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {toWatchMovies[i].Title}");
        }

        var choice = ReadNumber();

        if (choice < 1 || choice > toWatchMovies.Length)
        {
            Console.WriteLine("Invalid number!");
            Pause();
            return;
        }

        var selectedMovie = toWatchMovies[choice - 1];

        var isMarked = await service.MarkAsWatchedAsync(selectedMovie.Id);

        if (!isMarked)
        {
            Console.WriteLine("Movie not found!");
            Pause();
            return;
        }

        Console.WriteLine("Movie marked as watched!");
        Pause();
    }

    static async Task EditMovieTitle(MovieService service)
    {
        var movies = await service.GetAllMoviesAsync();

        if (movies.Length == 0)
        {
            Console.WriteLine("No movies to edit!");
            Pause();
            return;
        }

        Console.WriteLine("Select a movie to edit: ");

        for (int i = 0; i < movies.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {movies[i].Title}");
        }
        
        var choice = ReadNumber();

        if (choice < 1 || choice > movies.Length)
        {
            Console.WriteLine("Invalid number!");
            Pause();
            return;
        }

        var selectedMovie = movies[choice - 1];

        Console.WriteLine("Enter new title:");
        var newTitle = Console.ReadLine();
        
        var isUpdated = await service.EditMovieTitleAsync(selectedMovie.Id, newTitle ?? string.Empty);

        if (!isUpdated)
        {
            Console.WriteLine("Title cannot be empty or movie was not found!");
            Pause();
            return;
        }

        Console.WriteLine("Movie title updated!");
        Pause();
    }

    static int ReadNumber()
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

    static void Pause()
    {
        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
    }
}