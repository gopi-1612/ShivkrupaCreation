using System.ComponentModel.DataAnnotations;

namespace KurtiWholesale.Api.Models;

public class ContactRequestDto
{
    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string Message { get; set; } = string.Empty;
}

public record ContactMessageDto(
    int Id,
    string Name,
    string Email,
    string Phone,
    string Message,
    bool IsRead,
    DateTime SubmittedAtUtc
);
