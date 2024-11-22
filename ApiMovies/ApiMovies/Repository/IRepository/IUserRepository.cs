using ApiMovies.Models;
using ApiMovies.Models.Dtos;

namespace ApiMovies.Repository.IRepository
{
    public interface IUserRepository
    {
        ICollection<AppUser> GetUsers();
        AppUser GetUserById(string id);
        bool IsUniqueUser(string username);
        Task<LoginResponseDto> Login(LoginUserDto loginUserDto);
        Task<UserDataDto> Register(CreateUserDto createUserDto);
    }
}
