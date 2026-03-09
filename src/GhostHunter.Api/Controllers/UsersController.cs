using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GhostHunter.Core.Entities;
using GhostHunter.Infrastructure.Data;

namespace GhostHunter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    
    public UsersController(AppDbContext db)
    {
        _db = db;
    }
    
    // GET /api/users
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        return await _db.Users.ToListAsync();
    }
    
    // GET /api/users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetById(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();
        return user;
    }
    
    // POST /api/users
    [HttpPost]
    public async Task<ActionResult<User>> Create(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.PasswordHash = "temp-hash"; // We'll add proper auth later
        
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }
}
