using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShivkrupaCreation.API.DTOs.Brand;
using ShivkrupaCreation.API.DTOs.Common;
using ShivkrupaCreation.API.Models;
using ShivkrupaCreation.API.Services.Interfaces;

namespace ShivkrupaCreation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var brands = await _brandService.GetAllAsync(includeInactive);
            return Ok(ApiResponse<List<BrandDto>>.SuccessResponse(brands));
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _brandService.GetByIdAsync(id);
            return Ok(ApiResponse<BrandDto>.SuccessResponse(brand));
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody] BrandCreateDto dto)
        {
            var brand = await _brandService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = brand.Id },
                ApiResponse<BrandDto>.SuccessResponse(brand, "Brand created."));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] BrandUpdateDto dto)
        {
            var brand = await _brandService.UpdateAsync(id, dto);
            return Ok(ApiResponse<BrandDto>.SuccessResponse(brand, "Brand updated."));
        }

        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> SetStatus(int id, [FromQuery] bool isActive)
        {
            await _brandService.SetActiveStatusAsync(id, isActive);
            return Ok(ApiResponse<object>.SuccessResponse(new { }, isActive ? "Brand activated." : "Brand deactivated."));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await _brandService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Brand deleted."));
        }
    }
}
