using Microsoft.AspNetCore.Identity;

namespace ShivkrupaCreation.API.Models
{
    /// <summary>
    /// Extends ASP.NET Core Identity user with Shivkrupa Creation specific
    /// customer/business profile fields. Roles (Admin, Customer) are managed
    /// via IdentityRole and the AspNetUserRoles join table.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        // Wholesale customers usually represent a business/shop
        public string? BusinessName { get; set; }
        public string? GstNumber { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PinCode { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";
    }
}
