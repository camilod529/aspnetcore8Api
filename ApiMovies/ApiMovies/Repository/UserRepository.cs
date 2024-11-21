using ApiMovies.Data;
using ApiMovies.Models;
using ApiMovies.Models.Dtos;
using ApiMovies.Repository.IRepository;
using XSystem.Security.Cryptography;

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

        public async Task<User> Register(CreateUserDto createUserDto)
        {
            var encriptedPassword = getmd5(createUserDto.Password);

            User user = new()
            {
                UserName = createUserDto.UserName,
                Password = encriptedPassword,
                Name = createUserDto.Name,
                Role = createUserDto.Role,
                CreatedAt = DateTime.Now,
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
             user.Password = encriptedPassword;
            return user;
        }

        // Encrypt password
        private static string getmd5(string password)
        {
            MD5CryptoServiceProvider x = new();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(password);
            data = x.ComputeHash(data);
            string res = "";
            for (int i = 0; i < data.Length; i++)
            {
                res += data[i].ToString("x2").ToLower();
            }
            return res;
        }
    }
}
