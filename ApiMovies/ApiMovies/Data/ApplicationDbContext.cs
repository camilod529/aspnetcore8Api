using ApiMovies.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiMovies.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Todas las entidades (modelos)
        public DbSet<Category> Categories { get; set; }
        public DbSet<Movie> Movies { get; set; }
    }
}
