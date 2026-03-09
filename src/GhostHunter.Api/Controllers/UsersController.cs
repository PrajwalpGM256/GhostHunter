using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GhostHunter.Core.Entities;
using GhostHunter.Infrastructure.Data;
using GhostHunter.Api.Extensions;

namespace GhostHunter.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("me")]
    public async Task<ActionResult<User>> GetCurrentUser()
    {
        var userId = User.GetUserId();
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return NotFound();
        return user;
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateCurrentUser(UpdateUserRequest request)
    {
        var userId = User.GetUserId();
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return NotFound();

        if (request.EmailNotifications.HasValue)
            user.EmailNotifications = request.EmailNotifications.Value;

        if (!string.IsNullOrWhiteSpace(request.TelegramChatId))
            user.TelegramChatId = request.TelegramChatId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteCurrentUser()
    {
        var userId = User.GetUserId();
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return NotFound();

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public class UpdateUserRequest
{
    public bool? EmailNotifications { get; set; }
    public string? TelegramChatId { get; set; }
}
