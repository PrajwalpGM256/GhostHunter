using Microsoft.AspNetCore.Mvc;
using GhostHunter.Core.Entities;
using GhostHunter.Core.Interfaces;

namespace GhostHunter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScrapeLogsController : ControllerBase
{
    private readonly IScrapeLogService _scrapeLogService;

    public ScrapeLogsController(IScrapeLogService scrapeLogService)
    {
        _scrapeLogService = scrapeLogService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ScrapeLog>>> GetAll([FromQuery] int? watchId, [FromQuery] string? status)
    {
        return await _scrapeLogService.GetAllAsync(watchId, status);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ScrapeLog>> GetById(int id)
    {
        var log = await _scrapeLogService.GetByIdAsync(id);
        if (log == null) return NotFound();
        return log;
    }

    [HttpGet("watch/{watchId}")]
    public async Task<ActionResult<List<ScrapeLog>>> GetByWatch(int watchId)
    {
        return await _scrapeLogService.GetByWatchAsync(watchId);
    }

    [HttpGet("watch/{watchId}/latest")]
    public async Task<ActionResult<ScrapeLog>> GetLatestByWatch(int watchId)
    {
        var log = await _scrapeLogService.GetLatestByWatchAsync(watchId);
        if (log == null) return NotFound();
        return log;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _scrapeLogService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpDelete("watch/{watchId}")]
    public async Task<IActionResult> DeleteByWatch(int watchId)
    {
        var count = await _scrapeLogService.DeleteByWatchAsync(watchId);
        return Ok(new { DeletedCount = count });
    }
}
