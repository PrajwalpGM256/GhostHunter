using Microsoft.AspNetCore.Mvc;
using GhostHunter.Core.Entities;
using GhostHunter.Core.Interfaces;

namespace GhostHunter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobPostsController : ControllerBase
{
    private readonly IJobPostService _jobPostService;

    public JobPostsController(IJobPostService jobPostService)
    {
        _jobPostService = jobPostService;
    }

    [HttpGet]
    public async Task<ActionResult<List<JobPost>>> GetAll([FromQuery] int? watchId, [FromQuery] string? search)
    {
        return await _jobPostService.GetAllAsync(watchId, search);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobPost>> GetById(int id)
    {
        var post = await _jobPostService.GetByIdAsync(id);
        if (post == null) return NotFound();
        return post;
    }

    [HttpGet("watch/{watchId}")]
    public async Task<ActionResult<List<JobPost>>> GetByWatch(int watchId)
    {
        return await _jobPostService.GetByWatchAsync(watchId);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _jobPostService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpDelete("watch/{watchId}")]
    public async Task<IActionResult> DeleteByWatch(int watchId)
    {
        var count = await _jobPostService.DeleteByWatchAsync(watchId);
        return Ok(new { DeletedCount = count });
    }
}
