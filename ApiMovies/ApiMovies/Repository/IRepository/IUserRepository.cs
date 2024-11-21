using ApiMovies.Models;
using ApiMovies.Models.Dtos;

namespace ApiMovies.Repository.IRepository
{
    public interface IuserRepository
    {
        ICollection<User> GetUsers();
        User GetUserById(int id);
        bool IsUniqueUser(string username);
        Task<LoginResponseDto> Login(LoginUserDto loginUserDto);
        Task<UserDataDto> Register(CreateUserDto createUserDto);
    }
}
