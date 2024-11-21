namespace ApiMovies.Models.Dtos
{
    public class LoginResponseDto
    {
        public UserDataDto User {  get; set; }
        public string Role { get; set; }
        public string token { get; set; }
    }
}
