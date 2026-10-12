using KurtiWholesale.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShivkrupaCreation.API.Data;

namespace KurtiWholesale.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<ContactController> _logger;

    public ContactController(ApplicationDbContext db, ILogger<ContactController> logger)
    {
        _db = db;
        _logger = logger;
    }

    private static ContactMessageDto ToDto(ContactMessage m) => new(
        m.Id, m.Name, m.Email, m.Phone, m.Message, m.IsRead, m.SubmittedAtUtc);

    // POST /api/contact  (public)
    [HttpPost]
    public async Task<IActionResult> SubmitContact(ContactRequestDto request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var message = new ContactMessage
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone.Trim(),
            Message = request.Message.Trim(),
            SubmittedAtUtc = DateTime.UtcNow
        };

        _db.ContactMessages.Add(message);
        await _db.SaveChangesAsync();

        _logger.LogInformation("New wholesale inquiry received from {Email}", message.Email);

        return Ok(new { success = true, message = "Thank you — our wholesale team will reach out within 1 business day." });
    }

    // GET /api/contact  (Admin only)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<ContactMessageDto>>> GetMessages()
    {
        var messages = await _db.ContactMessages.OrderByDescending(m => m.SubmittedAtUtc).ToListAsync();
        return Ok(messages.Select(ToDto));
    }

    // PATCH /api/contact/5/read  (Admin only)
    [HttpPatch("{id:int}/read")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MarkRead(int id)
    {
        var message = await _db.ContactMessages.FindAsync(id);
        if (message is null) return NotFound();

        message.IsRead = true;
        await _db.SaveChangesAsync();
        return Ok(ToDto(message));
    }

    // DELETE /api/contact/5  (Admin only)
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        var message = await _db.ContactMessages.FindAsync(id);
        if (message is null) return NotFound();

        _db.ContactMessages.Remove(message);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
