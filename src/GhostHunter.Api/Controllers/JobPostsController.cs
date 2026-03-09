using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GhostHunter.Core.Entities;
using GhostHunter.Core.Interfaces;
using GhostHunter.Api.Extensions;

namespace GhostHunter.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class JobPostsController : ControllerBase
{
    private readonly IJobPostService _jobPostService;
    private readonly IWatchService _watchService;

    public JobPostsController(IJobPostService jobPostService, IWatchService watchService)
    {
        _jobPostService = jobPostService;
        _watchService = watchService;
    }

    [HttpGet]
    public async Task<ActionResult<List<JobPost>>> GetAll([FromQuery] int? watchId, [FromQuery] string? search)
    {
        var userId = User.GetUserId();
        var userWatches = await _watchService.GetAllAsync(userId);
        var userWatchIds = userWatches.Select(w => w.Id).ToList();

        var posts = await _jobPostService.GetAllAsync(watchId, search);
        return posts.Where(p => userWatchIds.Contains(p.WatchId)).ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobPost>> GetById(int id)
    {
        var post = await _jobPostService.GetByIdAsync(id);
        if (post == null) return NotFound();

        var watch = await _watchService.GetByIdAsync(post.WatchId);
        if (watch == null || watch.UserId != User.GetUserId()) return Forbid();

        return post;
    }

    [HttpGet("watch/{watchId}")]
    public async Task<ActionResult<List<JobPost>>> GetByWatch(int watchId)
    {
        var watch = await _watchService.GetByIdAsync(watchId);
        if (watch == null) return NotFound();
        if (watch.UserId != User.GetUserId()) return Forbid();

        return await _jobPostService.GetByWatchAsync(watchId);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _jobPostService.GetByIdAsync(id);
        if (post == null) return NotFound();

        var watch = await _watchService.GetByIdAsync(post.WatchId);
        if (watch == null || watch.UserId != User.GetUserId()) return Forbid();

        await _jobPostService.DeleteAsync(id);
        return NoContent();
    }

    [HttpDelete("watch/{watchId}")]
    public async Task<IActionResult> DeleteByWatch(int watchId)
    {
        var watch = await _watchService.GetByIdAsync(watchId);
        if (watch == null) return NotFound();
        if (watch.UserId != User.GetUserId()) return Forbid();

        var count = await _jobPostService.DeleteByWatchAsync(watchId);
        return Ok(new { DeletedCount = count });
    }
}
