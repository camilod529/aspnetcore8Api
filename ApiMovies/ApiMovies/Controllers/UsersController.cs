﻿using ApiMovies.Models.Dtos;
using ApiMovies.Repository;
using ApiMovies.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiMovies.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategories()
        {
            var users = _userRepository.GetUsers();
            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(_mapper.Map<UserDto>(user));
            }
            return Ok(userDtos);
        }
    }
}