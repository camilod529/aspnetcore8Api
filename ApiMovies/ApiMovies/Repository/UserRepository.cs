using ApiMovies.Data;
using ApiMovies.Models;
using ApiMovies.Models.Dtos;
using ApiMovies.Repository.IRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XSystem.Security.Cryptography;

namespace ApiMovies.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly string _key;
        public UserRepository(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _key = config.GetValue<string>("ApiSettings:secret");
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

        public async Task<LoginResponseDto> Login(LoginUserDto loginUserDto)
        {
            var encryptedPassword = Getmd5(loginUserDto.Password);
            var user = _db.Users.FirstOrDefault(u => u.UserName == loginUserDto.UserName && u.Password == encryptedPassword);

            // Validate if user not correct combination of username and password
            if (user == null)
            {
                return new LoginResponseDto()
                {
                    Token = "",
                    User = null,
                };
            }

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, user.Role),
                ]),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            LoginResponseDto loginResponseDto = new()
            {
                Token = jwtTokenHandler.WriteToken(token),
                User = user,
            };

            return loginResponseDto;
        }

        public async Task<User> Register(CreateUserDto createUserDto)
        {
            var encryptedPassword = Getmd5(createUserDto.Password);

            User user = new()
            {
                UserName = createUserDto.UserName,
                Password = encryptedPassword,
                Name = createUserDto.Name,
                Role = createUserDto.Role,
                CreatedAt = DateTime.Now,
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
             user.Password = encryptedPassword;
            return user;
        }

        // Encrypt password
        private static string Getmd5(string password)
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
