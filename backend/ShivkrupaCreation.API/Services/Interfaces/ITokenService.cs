using ShivkrupaCreation.API.Models;
using System.Security.Claims;

namespace ShivkrupaCreation.API.Services.Interfaces
{
    public interface ITokenService
    {
        (string token, DateTime expiresAt) GenerateAccessToken(ApplicationUser user, IList<string> roles);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
