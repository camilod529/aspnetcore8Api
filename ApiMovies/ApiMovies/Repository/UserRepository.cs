using ApiMovies.Data;
using ApiMovies.Models;
using ApiMovies.Models.Dtos;
using ApiMovies.Repository.IRepository;

namespace ApiMovies.Repository
{
    public class UserRepository : IuserRepository
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public User GetUserById(int id)
        {
            return _db.Users.FirstOrDefault(x => x.Id == id);
        }

        public ICollection<User> GetUsers()
        {
            return _db.Users.OrderBy(u => u.Name).ToList();
        }

        public bool IsUniqueUser(string username)
        {
            var user = _db.Users.FirstOrDefault(u => u.UserName == username);
            return user == null;
        }

        public Task<LoginResponseDto> Login(LoginUserDto loginUserDto)
        {
            throw new NotImplementedException();
        }

        public Task<UserDataDto> Register(CreateUserDto createUserDto)
        {
            throw new NotImplementedException();
        }
    }
}
