using ApiMovies.Models;
using ApiMovies.Models.Dtos;
using ApiMovies.Repository;
using ApiMovies.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiMovies.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        protected ApiResponses _apiResponses;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _apiResponses = new();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsers()
        {
            var users = _userRepository.GetUsers();
            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(_mapper.Map<UserDto>(user));
            }
            return Ok(userDtos);
        }

        [HttpGet("{id:int}", Name = "GetUserById")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUserById(int id)
        {
            var user = _userRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = _mapper.Map<UserDto>(user);

            return Ok(userDto);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto createUserDto)
        {
            bool isUnique = _userRepository.IsUniqueUser(createUserDto.UserName);
            if (!isUnique)
            {
                _apiResponses.StatusCode = HttpStatusCode.BadRequest;
                _apiResponses.IsSuccess = false;
                _apiResponses.ErrorMessages.Add("User already exists");
                return BadRequest(_apiResponses);
            }

            var user = await _userRepository.Register(createUserDto);
            if (user == null)
            {
                _apiResponses.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponses.IsSuccess = false;
                _apiResponses.ErrorMessages.Add("User creation failed");
                return StatusCode(500, _apiResponses);
            }

            _apiResponses.StatusCode = HttpStatusCode.Created;
            _apiResponses.IsSuccess = true;
            _apiResponses.Result = user;
            return CreatedAtRoute("GetUserById", new { id = user.Id }, _apiResponses);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserDto loginUserDto)
        {
            var loginResponse = await _userRepository.Login(loginUserDto);

            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token)) 
            {
                _apiResponses.StatusCode = HttpStatusCode.BadRequest;
                _apiResponses.IsSuccess = false;
                _apiResponses.ErrorMessages.Add("Login failed");
                return BadRequest(_apiResponses);
            }
            _apiResponses.StatusCode = HttpStatusCode.OK;
            _apiResponses.IsSuccess = true;
            _apiResponses.Result = loginResponse;
            return Ok(_apiResponses);
        }
    }
}
