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
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Select status:");
            Console.WriteLine("1. To Watch");
            Console.WriteLine("2. Watched");

            var statusInput = Console.ReadLine();
            MovieStatus status;

            switch (statusInput)
            {
                case "1":
                    status = MovieStatus.ToWatch;
                    break;
                case "2":
                    status = MovieStatus.Watched;
                    break;
                default:
                    Console.WriteLine("Invalid status!");
                    Console.ReadLine();
                    return;
            }

            var movie = new Movie(titleInput, status);
            _movies.Add(movie);

            Console.WriteLine("Movie added!");
            Console.ReadLine();
        }

        public void ShowAll()
        {
            if (_movies.Count == 0)
            {
                Console.WriteLine("No movies yet!");
                Console.ReadLine();
                return;
            }
            
            Console.WriteLine("To Watch:");
            foreach (var movie in _movies.Where(m => m.Status == MovieStatus.ToWatch))
            {
                Console.WriteLine(movie.Title);
            }
                
            Console.WriteLine("\nWatched:");
            foreach (var movie in _movies.Where(m => m.Status == MovieStatus.Watched))
            {
                Console.WriteLine(movie.Title);
            }
                
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        public void Delete()
        {
            if (_movies.Count == 0)
            {
                Console.WriteLine("No movies to delete!");
                Console.ReadLine();
                return;
            }
            
            Console.WriteLine("Select a movie to delete:");
            
            for (var i = 0; i < _movies.Count; i++)
            {
                Console.WriteLine($"{i+1}. {_movies[i].Title}");
            }
            var input = Console.ReadLine();
            
            if (!int.TryParse(input, out var choice))
            {
                Console.WriteLine("Invalid input! Please enter a number.");
                Console.ReadLine();
                return;
            }
            
            if (choice < 1 || choice > _movies.Count)
            {
                Console.WriteLine("Invalid number!");
                Console.ReadLine();
                return;
            }
            
            var removedMovie = _movies[choice - 1];
            _movies.RemoveAt(choice - 1);
            
            Console.WriteLine($"Movie \"{removedMovie.Title}\" removed!");
            Console.ReadLine();
        }

        public void MarkAsWatched()
        {
            var toWatchMovies =  _movies.Where(m => m.Status == MovieStatus.ToWatch).ToList();

            if (toWatchMovies.Count == 0)
            {
                Console.WriteLine("No movies to mark as watched!");
                Console.ReadLine();
                return;
            }
            
            Console.WriteLine("Select a movie to mark as watched:");

            for (int i = 0; i < toWatchMovies.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {toWatchMovies[i].Title}");
            }
            
            var input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("Invalid input! Please enter a number.");
                Console.ReadLine();
                return;
            }
            
            if (choice < 1 || choice > toWatchMovies.Count)
            {
                Console.WriteLine("Invalid number!");
                Console.ReadLine();
                return;
            }
            
            var selectedMovie = toWatchMovies[choice - 1];
            selectedMovie.Status = MovieStatus.Watched;

            Console.WriteLine($"Movie \"{selectedMovie.Title}\" marked as watched!");
            Console.ReadLine();
        }
}