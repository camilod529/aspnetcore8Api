using ApiMovies.Models;
using ApiMovies.Models.Dtos;
using ApiMovies.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiMovies.Controllers.V1
{
    [Route("api/v{version:apiVersion}/movies")]
    [ApiController]
    [ApiVersion("1.0")]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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

            var movieDto = _mapper.Map<MovieDto>(movie);

            return Ok(movieDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MovieDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateMovie([FromForm] CreateMovieDto createMovieDto)
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

            //if (!_movieRepository.CreateMovie(movie))
            //{
            //    ModelState.AddModelError("", $"Algo salio mal guardando el registro {movie.Name}");
            //    return BadRequest(ModelState);
            //}
            // File upload
            if (createMovieDto.Image != null)
            {
                string fileName = movie.Id + Guid.NewGuid().ToString() + Path.GetExtension(createMovieDto.Image.FileName);
                string fileRoute = @"Media\MovieImages\" + fileName;
                string fileRouteToSave = Path.Combine(Directory.GetCurrentDirectory(), fileRoute);
                FileInfo file = new FileInfo(fileRouteToSave);
                if (file.Exists)
                {
                    file.Delete();
                }
                using (var fileStream = new FileStream(fileRouteToSave, FileMode.Create))
                {
                    createMovieDto.Image.CopyTo(fileStream);
                }

                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                movie.ImageRoute = baseUrl + "/MovieImages/" + fileName;
                movie.LocalImageRoute = fileRoute;
            }
            else
            {
                movie.ImageRoute = "https://placehold.co/600x400";
            }

            _movieRepository.CreateMovie(movie);

            return CreatedAtRoute("GetMovieById", new { id = movie.Id }, movie);
        }

        [HttpPatch("{id:int}", Name = "UpdatePatchMovie")]
        [Authorize(Roles = "Admin")]
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

        [HttpDelete("{id:int}", Name = "DeleteMovie")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteMovie(int id)
        {
            if (!_movieRepository.MovieExists(id))
            {
                return NotFound();
            }

            var category = _movieRepository.GetMovieById(id);

            if (!_movieRepository.DeleteMovie(category))
            {
                ModelState.AddModelError("message", $"Algo salio mal borrando la pelicula {category.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpGet("GetMoviesInCategory/{categoryId:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetMoviesInCategory(int categoryId)
        {
            var movies = _movieRepository.GetMoviesByCategory(categoryId);

            if (movies == null)
            {
                return NotFound();
            }

            var moviesDto = new List<MovieDto>();
            foreach (var movie in movies)
            {
                moviesDto.Add(_mapper.Map<MovieDto>(movie));
            }

            return Ok(moviesDto);
        }

        // Asi se crean peticiones para detectar queryParams
        [HttpGet("search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SearchMovies(string query)
        {
            try
            {
                var res = _movieRepository.SearchMovie(query);
                if (res.Count != 0)
                {
                    return Ok(res);
                }
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error buscando peliculas por el termino {query}");
            }
        }
    }
}
