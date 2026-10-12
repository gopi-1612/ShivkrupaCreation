using ShivkrupaCreation.API.DTOs.Auth;

namespace ShivkrupaCreation.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto);
        Task RevokeRefreshTokenAsync(string userId, string refreshToken);
    }
}
