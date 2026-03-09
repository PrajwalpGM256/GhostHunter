using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GhostHunter.Core.Entities;
using GhostHunter.Core.Interfaces;
using GhostHunter.Api.Extensions;

namespace GhostHunter.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AlertsController : ControllerBase
{
    private readonly IAlertService _alertService;

    public AlertsController(IAlertService alertService)
    {
        _alertService = alertService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Alert>>> GetAll([FromQuery] string? status)
    {
        var userId = User.GetUserId();
        return await _alertService.GetAllAsync(userId, status);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Alert>> GetById(int id)
    {
        var alert = await _alertService.GetByIdAsync(id);
        if (alert == null) return NotFound();
        if (alert.UserId != User.GetUserId()) return Forbid();
        return alert;
    }

    [HttpGet("watch/{watchId}")]
    public async Task<ActionResult<List<Alert>>> GetByWatch(int watchId)
    {
        var userId = User.GetUserId();
        var alerts = await _alertService.GetByWatchAsync(watchId);
        return alerts.Where(a => a.UserId == userId).ToList();
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        var alert = await _alertService.GetByIdAsync(id);
        if (alert == null) return NotFound();
        if (alert.UserId != User.GetUserId()) return Forbid();

        await _alertService.UpdateStatusAsync(id, request.Status, request.ErrorMessage);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var alert = await _alertService.GetByIdAsync(id);
        if (alert == null) return NotFound();
        if (alert.UserId != User.GetUserId()) return Forbid();

        await _alertService.DeleteAsync(id);
        return NoContent();
    }
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}
