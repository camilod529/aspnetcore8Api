using ApiMovies.Models;
using ApiMovies.Models.Dtos;
using ApiMovies.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiMovies.Controllers
{
    //[Route("api/[controller]")] // Opcion estatica
    [Route("api/v{version:apiVersion}/category")] // opcion dinamica
    [ApiController]
    //[ResponseCache(Duration = 20)] // A nivel de controlador, 20 segundos dura el chache
    //[Authorize(Roles = "admin")] // Añade autenticacion a nivel de controlador
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        // [AllowAnonymous] // Permite usar el endpoint aunque la clase este protegida
        //[ResponseCache(Duration = 20)] /*Reduce la carga del servidor*/
        [ResponseCache(CacheProfileName = "DefaultCache")] /*Cache de perfil*/
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [MapToApiVersion("1.0")]
        public IActionResult GetCategories()
        {
            var categories = _categoryRepository.GetAll();
            var categoriesDto = new List<CategoryDto>();
            foreach (var category in categories)
            {
                categoriesDto.Add(_mapper.Map<CategoryDto>(category));
            }
            return Ok(categoriesDto);
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public IEnumerable<string> Get()
        {
            return ["Valor1", "valor2"];
        }

        [HttpGet("{id:int}", Name = "GetCategoryById")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCategoryById(int id)
        {
            var category = _categoryRepository.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);

            return Ok(categoryDto);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (createCategoryDto == null)
            {
                return BadRequest();
            }

            if (_categoryRepository.CategoryExists(createCategoryDto.Name))
            {
                ModelState.AddModelError("message", $"La categoria {createCategoryDto.Name} ya existe");
                return BadRequest(ModelState);
            }

            var category = _mapper.Map<Category>(createCategoryDto);

            if (!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro {category.Name}");
                return BadRequest(ModelState);
            }

            return CreatedAtRoute("GetCategoryById", new { id = category.Id }, category);
        }

        [HttpPatch("{id:int}", Name = "UpdatePatchCategory")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePatchCategory(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (updateCategoryDto == null || updateCategoryDto.Id != id)
            {
                return BadRequest(ModelState);
            }

            if (_categoryRepository.CategoryExists(updateCategoryDto.Name))
            {
                ModelState.AddModelError("message", $"Ya existe una categoria con el nombre {updateCategoryDto.Name}");
                return BadRequest(ModelState);
            }

            var existingCategory = _categoryRepository.GetCategoryById(id);

            if (existingCategory == null)
            {
                return NotFound();
            }

            _mapper.Map(updateCategoryDto, existingCategory);
            existingCategory.UpdatedAt = DateTime.Now;

            if (!_categoryRepository.UpdateCategory(existingCategory))
            {
                ModelState.AddModelError("message", $"Algo salio mal actualizando el registro {existingCategory.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdatePutCategory")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePutCategory(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (updateCategoryDto == null || updateCategoryDto.Id != id)
            {
                return BadRequest(ModelState);
            }

            if (_categoryRepository.CategoryExists(updateCategoryDto.Name))
            {
                ModelState.AddModelError("message", $"Ya existe una categoria con el nombre {updateCategoryDto.Name}");
                return BadRequest(ModelState);
            }

            var existingCategory = _categoryRepository.GetCategoryById(id);

            if (existingCategory == null)
            {
                return NotFound();
            }

            _mapper.Map(updateCategoryDto, existingCategory);
            existingCategory.UpdatedAt = DateTime.Now;

            if (!_categoryRepository.UpdateCategory(existingCategory))
            {
                ModelState.AddModelError("message", $"Algo salio mal actualizando el registro {existingCategory.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeletetCategory")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeletetCategory(int id)
        {
            if (!_categoryRepository.CategoryExists(id))
            {
                return NotFound();
            }

            var category = _categoryRepository.GetCategoryById(id);

            if (!_categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("message", $"Algo salio mal borrando la categoria {category.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
