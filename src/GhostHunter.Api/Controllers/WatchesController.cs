using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GhostHunter.Core.Entities;
using GhostHunter.Core.Interfaces;
using GhostHunter.Api.Extensions;

namespace GhostHunter.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WatchesController : ControllerBase
{
    private readonly IWatchService _watchService;

    public WatchesController(IWatchService watchService)
    {
        _watchService = watchService;
    }

    [HttpGet]
    public async Task<ActionResult<List<JobWatch>>> GetAll()
    {
        var userId = User.GetUserId();
        return await _watchService.GetAllAsync(userId);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobWatch>> GetById(int id)
    {
        var watch = await _watchService.GetByIdAsync(id);
        if (watch == null) return NotFound();
        if (watch.UserId != User.GetUserId()) return Forbid();
        return watch;
    }

    [HttpGet("active")]
    public async Task<ActionResult<List<JobWatch>>> GetActive()
    {
        var userId = User.GetUserId();
        var watches = await _watchService.GetActiveAsync();
        return watches.Where(w => w.UserId == userId).ToList();
    }

    [HttpPost]
    public async Task<ActionResult<JobWatch>> Create(CreateWatchRequest request)
    {
        var watch = new JobWatch
        {
            UserId = User.GetUserId(),
            Name = request.Name,
            Url = request.Url,
            JobTitleSelector = request.JobTitleSelector,
            CompanySelector = request.CompanySelector,
            LinkSelector = request.LinkSelector,
            LocationSelector = request.LocationSelector,
            IncludeKeywords = request.IncludeKeywords ?? "[]",
            ExcludeKeywords = request.ExcludeKeywords ?? "[]",
            CheckIntervalMinutes = request.CheckIntervalMinutes ?? 60,
            IsActive = request.IsActive ?? true
        };

        await _watchService.CreateAsync(watch);
        return CreatedAtAction(nameof(GetById), new { id = watch.Id }, watch);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateWatchRequest request)
    {
        var watch = await _watchService.GetByIdAsync(id);
        if (watch == null) return NotFound();
        if (watch.UserId != User.GetUserId()) return Forbid();

        watch.Name = request.Name;
        watch.Url = request.Url;
        watch.JobTitleSelector = request.JobTitleSelector;
        watch.CompanySelector = request.CompanySelector;
        watch.LinkSelector = request.LinkSelector;
        watch.LocationSelector = request.LocationSelector;
        watch.IncludeKeywords = request.IncludeKeywords ?? watch.IncludeKeywords;
        watch.ExcludeKeywords = request.ExcludeKeywords ?? watch.ExcludeKeywords;
        watch.CheckIntervalMinutes = request.CheckIntervalMinutes ?? watch.CheckIntervalMinutes;
        watch.IsActive = request.IsActive ?? watch.IsActive;

        await _watchService.UpdateAsync(watch);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var watch = await _watchService.GetByIdAsync(id);
        if (watch == null) return NotFound();
        if (watch.UserId != User.GetUserId()) return Forbid();

        await _watchService.DeleteAsync(id);
        return NoContent();
    }
}

public class CreateWatchRequest
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? JobTitleSelector { get; set; }
    public string? CompanySelector { get; set; }
    public string? LinkSelector { get; set; }
    public string? LocationSelector { get; set; }
    public string? IncludeKeywords { get; set; }
    public string? ExcludeKeywords { get; set; }
    public int? CheckIntervalMinutes { get; set; }
    public bool? IsActive { get; set; }
}
