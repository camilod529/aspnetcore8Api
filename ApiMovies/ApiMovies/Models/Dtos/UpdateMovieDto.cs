using System.ComponentModel.DataAnnotations.Schema;

namespace ApiMovies.Models.Dtos
{
    public class UpdateMovieDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public string ImageRoute { get; set; }
        public enum UpdateClassificationType
        {
            Seven, Thirteen, Sixteen, Eighteen
        }
        public UpdateClassificationType Classification { get; set; }
        //Relacion con Category
        public int CategoryId { get; set; }
    }
}
