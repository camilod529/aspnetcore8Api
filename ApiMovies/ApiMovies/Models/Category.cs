using System.ComponentModel.DataAnnotations;

namespace ApiMovies.Models
{
    public class Category
    {
        [Key] // Este decorador indica llave primaria autoincremental
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
}
