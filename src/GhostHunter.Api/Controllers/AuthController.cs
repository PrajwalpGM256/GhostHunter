using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GhostHunter.Core.Interfaces;

namespace GhostHunter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthRequest request)
    {
        var user = await _authService.RegisterAsync(request.Email, request.Password);

        if (user == null)
            return BadRequest(new { Message = "Email already exists" });

        var token = _authService.GenerateToken(user);

        return Ok(new AuthResponse
        {
            Id = user.Id,
            Email = user.Email,
            Token = token
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var user = await _authService.LoginAsync(request.Email, request.Password);

        if (user == null)
            return Unauthorized(new { Message = "Invalid email or password" });

        var token = _authService.GenerateToken(user);

        return Ok(new AuthResponse
        {
            Id = user.Id,
            Email = user.Email,
            Token = token
        });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _authService.GetByIdAsync(userId);

        if (user == null)
            return NotFound();

        return Ok(new
        {
            user.Id,
            user.Email,
            user.EmailNotifications,
            user.CreatedAt
        });
    }
}

public class AuthRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
