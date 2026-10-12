namespace ShivkrupaCreation.API.DTOs.Auth
{
    public class RegisterDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }

        // Wholesale customer business details (optional at signup)
        public string? BusinessName { get; set; }
        public string? GstNumber { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RefreshTokenRequestDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? BusinessName { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAt { get; set; }
        public UserDto User { get; set; } = null!;
    }
}
