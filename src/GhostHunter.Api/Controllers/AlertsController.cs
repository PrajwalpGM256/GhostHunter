using Microsoft.AspNetCore.Mvc;
using GhostHunter.Core.Entities;
using GhostHunter.Core.Interfaces;

namespace GhostHunter.Api.Controllers;

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
    public async Task<ActionResult<List<Alert>>> GetAll([FromQuery] int? userId, [FromQuery] string? status)
    {
        return await _alertService.GetAllAsync(userId, status);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Alert>> GetById(int id)
    {
        var alert = await _alertService.GetByIdAsync(id);
        if (alert == null) return NotFound();
        return alert;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<Alert>>> GetByUser(int userId)
    {
        return await _alertService.GetByUserAsync(userId);
    }

    [HttpGet("watch/{watchId}")]
    public async Task<ActionResult<List<Alert>>> GetByWatch(int watchId)
    {
        return await _alertService.GetByWatchAsync(watchId);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        var alert = await _alertService.GetByIdAsync(id);
        if (alert == null) return NotFound();

        await _alertService.UpdateStatusAsync(id, request.Status, request.ErrorMessage);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _alertService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}
