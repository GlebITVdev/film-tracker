namespace Film_Tracker;

class Program
{
    static void Main(string[] args)
    {
        var movieService = new MovieService();
        
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to the Film-Tracker!");
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("1. Add movie");
            Console.WriteLine("2. Delete movie");
            Console.WriteLine("3. Mark as watched");
            Console.WriteLine("4. Show all movies");
            Console.WriteLine("5. Exit");
            
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    movieService.Add();
                    break;
                case "2":
                    movieService.Delete();
                    break;
                case "3":
                    movieService.ShowAll();
                    break;
                case "4":
                    movieService.MarkAsWatched();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Invalid option!");
                    Console.ReadLine();
                    break;
            }
        }
    }
}