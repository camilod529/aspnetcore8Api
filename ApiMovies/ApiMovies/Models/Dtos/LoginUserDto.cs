using System.ComponentModel.DataAnnotations;

namespace ApiMovies.Models.Dtos
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "El Nombre de usuario es obligatorio")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; }
    }
}
