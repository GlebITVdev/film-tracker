using System;
using FilmTracker.Core.Models;
using FilmTracker.Core.Services;

namespace FilmTracker.ConsoleApp;

class Program
{
    static void Main(string[] args)
    {
        var service = new MovieService();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to Film Tracker!");
            Console.WriteLine("1. Add movie");
            Console.WriteLine("2. Delete movie");
            Console.WriteLine("3. Show movies");
            Console.WriteLine("4. Mark as watched");
            Console.WriteLine("5. Exit");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddMovie(service);
                    break;
                case "2":
                    DeleteMovie(service);
                    break;
                case "3":
                    ShowMovies(service);
                    break;
                case "4":
                    MarkAsWatched(service);
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Invalid option!");
                    Pause();
                    break;
            }
        }
    }

    static void AddMovie(MovieService service)
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

        var isAdded = service.AddMovie(title ?? string.Empty, status);

        if (!isAdded)
        {
            Console.WriteLine("Title cannot be empty!");
            Pause();
            return;
        }

        Console.WriteLine("Movie added!");
        Pause();
    }

    static void ShowMovies(MovieService service)
    {
        var toWatch = service.GetToWatchMovies();
        var watched = service.GetWatchedMovies();

        Console.WriteLine("To Watch:");
        if (toWatch.Count == 0)
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
        if (watched.Count == 0)
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

    static void DeleteMovie(MovieService service)
    {
        var movies = service.GetAllMovies();

        if (movies.Count == 0)
        {
            Console.WriteLine("No movies to delete!");
            Pause();
            return;
        }

        Console.WriteLine("Select a movie to delete:");

        for (int i = 0; i < movies.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {movies[i].Title}");
        }

        var choice = ReadNumber();
        var isDeleted = service.DeleteMovie(choice - 1);

        if (!isDeleted)
        {
            Console.WriteLine("Invalid number!");
            Pause();
            return;
        }

        Console.WriteLine("Movie deleted!");
        Pause();
    }

    static void MarkAsWatched(MovieService service)
    {
        var toWatchMovies = service.GetToWatchMovies();

        if (toWatchMovies.Count == 0)
        {
            Console.WriteLine("No movies to mark as watched!");
            Pause();
            return;
        }

        Console.WriteLine("Select a movie to mark as watched:");

        for (int i = 0; i < toWatchMovies.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {toWatchMovies[i].Title}");
        }

        var choice = ReadNumber();
        var isMarked = service.MarkAsWatched(choice - 1);

        if (!isMarked)
        {
            Console.WriteLine("Invalid number!");
            Pause();
            return;
        }

        Console.WriteLine("Movie marked as watched!");
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