namespace ApiMovies.Models.Dtos
{
    public class LoginResponseDto
    {
        public User User {  get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
