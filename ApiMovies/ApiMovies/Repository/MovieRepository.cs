using ApiMovies.Data;
using ApiMovies.Models;
using ApiMovies.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiMovies.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly ApplicationDbContext _db;

        public MovieRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CreateMovie(Movie movie)
        {
            movie.CreatedAt = DateTime.Now;
            movie.UpdatedAt = null;
            _db.Movies.Add(movie);
            return Save();
        }

        public bool DeleteMovie(Movie movie)
        {
            _db.Movies.Remove(movie);
            return Save();
        }

        public ICollection<Movie> GetAll()
        {
            return _db.Movies.OrderBy(x => x.Name).ToList();
        }

        public Movie GetMovieById(int id)
        {
            return _db.Movies.FirstOrDefault(x => x.Id == id);
        }

        public ICollection<Movie> GetMoviesByCategory(int categoryId)
        {
            return _db.Movies.Include(ca => ca.Category).Where(ca => ca.CategoryId == categoryId).ToList();
        }
        public ICollection<Movie> SearchMovie(string name)
        {
            IQueryable<Movie> query = _db.Movies;

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Name.Contains(name) || x.Description.Contains(name));
            }

            return query.ToList();
        }
        public bool MovieExists(int id)
        {
            return _db.Movies.Any(x => x.Id == id);
        }
        public bool MovieExists(string name)
        {
            return _db.Movies.Any(y => y.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0;
        }
        public bool UpdateMovie(Movie movie)
        {
            movie.UpdatedAt = DateTime.Now;
            _db.Movies.Update(movie);
            return Save();
        }
    }
}
