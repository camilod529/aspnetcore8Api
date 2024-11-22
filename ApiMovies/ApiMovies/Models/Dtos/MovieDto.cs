using System.ComponentModel.DataAnnotations.Schema;

namespace ApiMovies.Models.Dtos
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public string? ImageRoute { get; set; }
        public string? LocalImageRoute { get; set; }
        public enum ClassificationTypeDto
        {
            Seven, Thirteen, Sixteen, Eighteen
        }
        public ClassificationTypeDto Classification { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //Relacion con Category
        public int CategoryId { get; set; }
    }
}
