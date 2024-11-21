using ApiMovies.Models;

namespace ApiMovies.Repository.IRepository
{
    public interface IMovieRepository
    {
        ICollection<Movie> GetAll();
        ICollection<Movie> GetMoviesByCategory(int categoryId);
        ICollection<Movie> SearchMovie(string name);
        Movie GetMovieById(int id);
        bool MovieExists(int id);
        bool MovieExists(string name);
        bool CreateMovie(Movie movie);
        bool UpdateMovie(Movie movie);
        bool DeleteMovie(Movie movie);
        bool Save();
    }
}
