using Microsoft.AspNetCore.Identity;

namespace ApiMovies.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
