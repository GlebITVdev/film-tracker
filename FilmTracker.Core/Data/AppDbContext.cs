using FilmTracker.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FilmTracker.Core.Data;

public class AppDbContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}