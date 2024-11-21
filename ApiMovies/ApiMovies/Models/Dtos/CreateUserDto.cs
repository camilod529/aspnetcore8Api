using System.ComponentModel.DataAnnotations;

namespace ApiMovies.Models.Dtos
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "El Nombre es obligatorio")]
        public string Name { get; set; }
        [Required(ErrorMessage = "El Nombre de usuario es obligatorio")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; }
        public string Role {  get; set; }
        
    }
}
