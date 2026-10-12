namespace ShivkrupaCreation.API.DTOs.Customer
{
    public class CustomerListItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? BusinessName { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CustomerDetailDto : CustomerListItemDto
    {
        public string? GstNumber { get; set; }
        public string? Address { get; set; }
        public string? PinCode { get; set; }
    }

    public class UpdateProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? BusinessName { get; set; }
        public string? GstNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PinCode { get; set; }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }

    public class UpdateCustomerStatusDto
    {
        public bool IsActive { get; set; }
    }
}
