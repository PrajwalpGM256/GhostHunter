using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GhostHunter.Core.Entities;
using GhostHunter.Core.Interfaces;
using GhostHunter.Api.Extensions;

namespace GhostHunter.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScrapeLogsController : ControllerBase
{
    private readonly IScrapeLogService _scrapeLogService;
    private readonly IWatchService _watchService;

    public ScrapeLogsController(IScrapeLogService scrapeLogService, IWatchService watchService)
    {
        _scrapeLogService = scrapeLogService;
        _watchService = watchService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ScrapeLog>>> GetAll([FromQuery] string? status)
    {
        var userId = User.GetUserId();
        var userWatches = await _watchService.GetAllAsync(userId);
        var userWatchIds = userWatches.Select(w => w.Id).ToList();

        var logs = await _scrapeLogService.GetAllAsync(null, status);
        return logs.Where(l => userWatchIds.Contains(l.WatchId)).ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ScrapeLog>> GetById(int id)
    {
        var log = await _scrapeLogService.GetByIdAsync(id);
        if (log == null) return NotFound();

        var watch = await _watchService.GetByIdAsync(log.WatchId);
        if (watch == null || watch.UserId != User.GetUserId()) return Forbid();

        return log;
    }

    [HttpGet("watch/{watchId}")]
    public async Task<ActionResult<List<ScrapeLog>>> GetByWatch(int watchId)
    {
        var watch = await _watchService.GetByIdAsync(watchId);
        if (watch == null) return NotFound();
        if (watch.UserId != User.GetUserId()) return Forbid();

        return await _scrapeLogService.GetByWatchAsync(watchId);
    }

    [HttpGet("watch/{watchId}/latest")]
    public async Task<ActionResult<ScrapeLog>> GetLatestByWatch(int watchId)
    {
        var watch = await _watchService.GetByIdAsync(watchId);
        if (watch == null) return NotFound();
        if (watch.UserId != User.GetUserId()) return Forbid();

        var log = await _scrapeLogService.GetLatestByWatchAsync(watchId);
        if (log == null) return NotFound();
        return log;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var log = await _scrapeLogService.GetByIdAsync(id);
        if (log == null) return NotFound();

        var watch = await _watchService.GetByIdAsync(log.WatchId);
        if (watch == null || watch.UserId != User.GetUserId()) return Forbid();

        await _scrapeLogService.DeleteAsync(id);
        return NoContent();
    }

    [HttpDelete("watch/{watchId}")]
    public async Task<IActionResult> DeleteByWatch(int watchId)
    {
        var watch = await _watchService.GetByIdAsync(watchId);
        if (watch == null) return NotFound();
        if (watch.UserId != User.GetUserId()) return Forbid();

        var count = await _scrapeLogService.DeleteByWatchAsync(watchId);
        return Ok(new { DeletedCount = count });
    }
}
