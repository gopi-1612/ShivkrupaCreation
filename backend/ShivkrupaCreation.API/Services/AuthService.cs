using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShivkrupaCreation.API.Data;
using ShivkrupaCreation.API.DTOs.Auth;
using ShivkrupaCreation.API.DTOs.Common;
using ShivkrupaCreation.API.Models;
using ShivkrupaCreation.API.Services.Interfaces;
using ShivkrupaCreation.API.Settings;

namespace ShivkrupaCreation.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db,
            ITokenService tokenService,
            IMapper mapper,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _db = db;
            _tokenService = tokenService;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing is not null)
            {
                throw ApiException.Conflict("An account with this email already exists.");
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                BusinessName = dto.BusinessName,
                GstNumber = dto.GstNumber,
                City = dto.City,
                State = dto.State,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                throw ApiException.BadRequest(
                    "Registration failed.",
                    result.Errors.Select(e => e.Description).ToList());
            }

            await _userManager.AddToRoleAsync(user, Roles.Customer);

            return await BuildAuthResponseAsync(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                throw ApiException.Unauthorized("Invalid email or password.");
            }

            if (!user.IsActive)
            {
                throw ApiException.Forbidden("Your account has been deactivated. Please contact support.");
            }

            return await BuildAuthResponseAsync(user);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken)
                ?? throw ApiException.Unauthorized("Invalid access token.");

            var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? throw ApiException.Unauthorized("Invalid access token.");

            var storedToken = await _db.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken && rt.UserId == userId);

            if (storedToken is null || !storedToken.IsActive)
            {
                throw ApiException.Unauthorized("Refresh token is invalid or has expired. Please log in again.");
            }

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw ApiException.Unauthorized("User no longer exists.");

            // rotate the refresh token: revoke old one, issue a new one
            storedToken.RevokedAt = DateTime.UtcNow;

            return await BuildAuthResponseAsync(user);
        }

        public async Task RevokeRefreshTokenAsync(string userId, string refreshToken)
        {
            var storedToken = await _db.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

            if (storedToken is not null && storedToken.IsActive)
            {
                storedToken.RevokedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }

        private async Task<AuthResponseDto> BuildAuthResponseAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var (accessToken, expiresAt) = _tokenService.GenerateAccessToken(user, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();

            _db.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
            });
            await _db.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = roles.ToList();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = expiresAt,
                User = userDto
            };
        }
    }
}
