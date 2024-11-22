using ApiMovies.Data;
using ApiMovies.Models;
using ApiMovies.Models.Dtos;
using ApiMovies.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        public UserRepository(ApplicationDbContext db, IConfiguration config, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _db = db;
            _key = config.GetValue<string>("ApiSettings:secret");
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public AppUser GetUserById(string id)
        {
            return _db.AppUsers.FirstOrDefault(x => x.Id == id);
        }

        public ICollection<AppUser> GetUsers()
        {
            return _db.AppUsers.OrderBy(u => u.UserName).ToList();
        }

        public bool IsUniqueUser(string username)
        {
            var user = _db.AppUsers.FirstOrDefault(u => u.UserName == username);
            return user == null;
        }

        public async Task<LoginResponseDto> Login(LoginUserDto loginUserDto)
        {
            //var encryptedPassword = Getmd5(loginUserDto.Password);
            var user = _db.AppUsers.FirstOrDefault(u => u.UserName.ToLower() == loginUserDto.UserName.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginUserDto.Password);

            // Validate if user not correct combination of username and password
            if (user == null || !isValid)
            {
                return new LoginResponseDto()
                {
                    Token = "",
                    User = null,
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                ]),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            LoginResponseDto loginResponseDto = new()
            {
                Token = jwtTokenHandler.WriteToken(token),
                User = _mapper.Map<UserDataDto>(user),
                Role = roles.FirstOrDefault(),
            };

            return loginResponseDto;
        }

        public async Task<UserDataDto> Register(CreateUserDto createUserDto)
        {
            // var encryptedPassword = Getmd5(createUserDto.Password);
            AppUser user = new()
            {
                UserName = createUserDto.UserName,
                Email = createUserDto.UserName,
                NormalizedEmail = createUserDto.UserName.ToUpper(),
                FullName = createUserDto.Name,
            };

            var result = await _userManager.CreateAsync(user, createUserDto.Password);
            if(result.Succeeded)
            {
                if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                }
                await _userManager.AddToRoleAsync(user, "Admin");
                var returnUser = _db.AppUsers.FirstOrDefault(u => u.UserName == createUserDto.UserName);
                return _mapper.Map<UserDataDto>(returnUser);
            }
            return new UserDataDto();
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
