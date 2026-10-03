using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using SalesInventoryManagement.Application.DTOs;
using SalesInventoryManagement.Application.Interfaces;

namespace SalesInventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IValidator<CreateCategoryDto> _validator;

        public CategoriesController(ICategoryService categoryService, IValidator<CreateCategoryDto> validator)
        {
            _categoryService = categoryService;
            _validator = validator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryDto>>> GetAll() =>
            Ok(await _categoryService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return category is null ? NotFound($"Category with id {id} not found") : Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create(CreateCategoryDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var created = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
    }
}
