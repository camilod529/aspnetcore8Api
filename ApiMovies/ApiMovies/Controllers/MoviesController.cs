using ApiMovies.Models;
using ApiMovies.Models.Dtos;
using ApiMovies.Repository;
using ApiMovies.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiMovies.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;

        public MoviesController(IMovieRepository movieRepository, IMapper mapper)
        {
            _movieRepository = movieRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetMovies()
        {
            var movies = _movieRepository.GetAll();
            var moviesDto = new List<MovieDto>();
            foreach (var movie in movies)
            {
                moviesDto.Add(_mapper.Map<MovieDto>(movie));
            }
            return Ok(moviesDto);
        }

        [HttpGet("{id:int}", Name = "GetMovieById")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetMovieById(int id)
        {
            var movie = _movieRepository.GetMovieById(id);

            if (movie == null)
            {
                return NotFound();
            }

            var movieDto = _mapper.Map<CategoryDto>(movie);

            return Ok(movieDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MovieDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateMovie([FromBody] CreateMovieDto createMovieDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (createMovieDto == null)
            {
                return BadRequest();
            }

            if (_movieRepository.MovieExists(createMovieDto.Name))
            {
                ModelState.AddModelError("message", $"La pelicula {createMovieDto.Name} ya existe");
                return BadRequest(ModelState);
            }

            var movie = _mapper.Map<Movie>(createMovieDto);

            if (!_movieRepository.CreateMovie(movie))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro {movie.Name}");
                return BadRequest(ModelState);
            }

            return CreatedAtRoute("GetMovieById", new { id = movie.Id }, movie);
        }

        [HttpPatch("{id:int}", Name = "UpdatePatchMovie")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePatchMovie(int id, [FromBody] UpdateMovieDto updateMovieDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (updateMovieDto == null || updateMovieDto.Id != id)
            {
                return BadRequest(ModelState);
            }

            if (_movieRepository.MovieExists(updateMovieDto.Name))
            {
                ModelState.AddModelError("message", $"Ya existe una pelicula con el nombre {updateMovieDto.Name}");
                return BadRequest(ModelState);
            }

            var existingMovie = _movieRepository.GetMovieById(id);

            if (existingMovie == null)
            {
                return NotFound();
            }

            _mapper.Map(updateMovieDto, existingMovie);
            existingMovie.UpdatedAt = DateTime.Now;

            if (!_movieRepository.UpdateMovie(existingMovie))
            {
                ModelState.AddModelError("message", $"Algo salio mal actualizando el registro {existingMovie.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
