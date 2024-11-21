using System.ComponentModel.DataAnnotations;

namespace ApiMovies.Models.Dtos
{
    public class UpdateCategoryDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "The max characters number is 100")]
        [MinLength(1, ErrorMessage = "The minimum character number is 1")]
        public string Name { get; set; }

    }
}
