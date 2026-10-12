using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShivkrupaCreation.API.DTOs.Common;
using ShivkrupaCreation.API.DTOs.Customer;
using ShivkrupaCreation.API.Models;
using ShivkrupaCreation.API.Services.Interfaces;
using System.Security.Claims;

namespace ShivkrupaCreation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // ---------- Admin: customer management ----------

        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search, [FromQuery] bool? isActive,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _customerService.GetAllAsync(search, isActive, pageNumber, pageSize);
            return Ok(ApiResponse<PagedResult<CustomerListItemDto>>.SuccessResponse(result));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetById(string id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            return Ok(ApiResponse<CustomerDetailDto>.SuccessResponse(customer));
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> SetStatus(string id, [FromBody] UpdateCustomerStatusDto dto)
        {
            await _customerService.SetActiveStatusAsync(id, dto.IsActive);
            return Ok(ApiResponse<object>.SuccessResponse(new { },
                dto.IsActive ? "Customer activated." : "Customer deactivated."));
        }

        // ---------- Self-service: logged-in customer's own profile ----------

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var profile = await _customerService.GetProfileAsync(userId);
            return Ok(ApiResponse<CustomerDetailDto>.SuccessResponse(profile));
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var profile = await _customerService.UpdateProfileAsync(userId, dto);
            return Ok(ApiResponse<CustomerDetailDto>.SuccessResponse(profile, "Profile updated."));
        }

        [HttpPost("me/change-password")]
        public async Task<IActionResult> ChangeMyPassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _customerService.ChangePasswordAsync(userId, dto);
            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Password changed successfully."));
        }
    }
}
