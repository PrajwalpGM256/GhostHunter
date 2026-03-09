using Microsoft.EntityFrameworkCore;
using GhostHunter.Core.Entities;
using GhostHunter.Core.Interfaces;
using GhostHunter.Infrastructure.Data;

namespace GhostHunter.Infrastructure.Services;

public class AlertService : IAlertService
{
    private readonly AppDbContext _db;

    public AlertService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Alert>> GetAllAsync(int? userId = null, string? status = null)
    {
        var query = _db.Alerts.AsQueryable();

        if (userId.HasValue)
            query = query.Where(a => a.UserId == userId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(a => a.Status == status);

        return await query.OrderByDescending(a => a.SentAt).ToListAsync();
    }

    public async Task<Alert?> GetByIdAsync(int id)
    {
        return await _db.Alerts.FindAsync(id);
    }

    public async Task<List<Alert>> GetByUserAsync(int userId)
    {
        return await _db.Alerts
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.SentAt)
            .ToListAsync();
    }

    public async Task<List<Alert>> GetByWatchAsync(int watchId)
    {
        return await _db.Alerts
            .Where(a => a.WatchId == watchId)
            .OrderByDescending(a => a.SentAt)
            .ToListAsync();
    }

    public async Task<Alert> CreateAsync(Alert alert)
    {
        _db.Alerts.Add(alert);
        await _db.SaveChangesAsync();
        return alert;
    }

    public async Task<List<Alert>> CreateForNewPostsAsync(int userId, int watchId, List<JobPost> newPosts)
    {
        var alerts = newPosts.Select(post => new Alert
        {
            UserId = userId,
            WatchId = watchId,
            JobPostId = post.Id,
            SentAt = DateTime.UtcNow,
            Status = "pending"
        }).ToList();

        if (alerts.Any())
        {
            _db.Alerts.AddRange(alerts);
            await _db.SaveChangesAsync();
        }

        return alerts;
    }

    public async Task UpdateStatusAsync(int id, string status, string? errorMessage = null)
    {
        var alert = await _db.Alerts.FindAsync(id);
        if (alert == null) return;

        alert.Status = status;
        if (errorMessage != null)
            alert.ErrorMessage = errorMessage;

        await _db.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var alert = await _db.Alerts.FindAsync(id);
        if (alert == null) return false;

        _db.Alerts.Remove(alert);
        await _db.SaveChangesAsync();
        return true;
    }
}
