using FilmTracker.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FilmTracker.Core.Data;

public class AppDbContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}