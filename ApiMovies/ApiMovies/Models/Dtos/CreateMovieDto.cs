using System.ComponentModel.DataAnnotations.Schema;

namespace ApiMovies.Models.Dtos
{
    public class CreateMovieDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public string? ImageRoute { get; set; }
        public IFormFile? Image { get; set; }
        public enum CreateClassificationType
        {
            Seven, Thirteen, Sixteen, Eighteen
        }
        public CreateClassificationType Classification { get; set; }
        //Relacion con Category
        public int CategoryId { get; set; }
    }
}
