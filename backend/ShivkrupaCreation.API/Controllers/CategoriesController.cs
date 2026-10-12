using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShivkrupaCreation.API.DTOs.Category;
using ShivkrupaCreation.API.DTOs.Common;
using ShivkrupaCreation.API.Models;
using ShivkrupaCreation.API.Services.Interfaces;

namespace ShivkrupaCreation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>Public: list categories. Admins can pass includeInactive=true to see deactivated ones.</summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var categories = await _categoryService.GetAllAsync(includeInactive);
            return Ok(ApiResponse<List<CategoryDto>>.SuccessResponse(categories));
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return Ok(ApiResponse<CategoryDto>.SuccessResponse(category));
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromForm] CategoryCreateDto dto)
        {
            var category = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = category.Id },
                ApiResponse<CategoryDto>.SuccessResponse(category, "Category created."));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(int id, [FromForm] CategoryUpdateDto dto)
        {
            var category = await _categoryService.UpdateAsync(id, dto);
            return Ok(ApiResponse<CategoryDto>.SuccessResponse(category, "Category updated."));
        }

        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> SetStatus(int id, [FromQuery] bool isActive)
        {
            await _categoryService.SetActiveStatusAsync(id, isActive);
            return Ok(ApiResponse<object>.SuccessResponse(new { }, isActive ? "Category activated." : "Category deactivated."));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Category deleted."));
        }
    }
}
