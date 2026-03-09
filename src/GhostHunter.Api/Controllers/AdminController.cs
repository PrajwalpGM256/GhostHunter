using Microsoft.AspNetCore.Mvc;
using GhostHunter.Infrastructure.Data;

namespace GhostHunter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        await DbSeeder.SeedAsync(_context);
        return Ok(new { Message = "Database seeded (skipped if data exists)" });
    }

    [HttpPost("reset-and-seed")]
    public async Task<IActionResult> ResetAndSeed()
    {
        await DbSeeder.ResetAndSeedAsync(_context);
        return Ok(new { Message = "Database reset and seeded successfully" });
    }
}
