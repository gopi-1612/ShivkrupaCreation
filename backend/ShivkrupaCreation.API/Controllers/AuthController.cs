using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShivkrupaCreation.API.DTOs.Auth;
using ShivkrupaCreation.API.DTOs.Common;
using ShivkrupaCreation.API.Services.Interfaces;
using System.Security.Claims;

namespace ShivkrupaCreation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>Register a new wholesale customer account.</summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Registration successful."));
        }

        /// <summary>Log in with email + password. Returns an access token and refresh token.</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Login successful."));
        }

        /// <summary>Exchange an expired access token + valid refresh token for a new pair.</summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Token refreshed."));
        }

        /// <summary>Revoke the current refresh token (logout on this device).</summary>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _authService.RevokeRefreshTokenAsync(userId, dto.RefreshToken);
            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Logged out successfully."));
        }
    }
}
