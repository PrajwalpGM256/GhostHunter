using Microsoft.EntityFrameworkCore;
using GhostHunter.Core.Entities;
using GhostHunter.Core.Interfaces;
using GhostHunter.Infrastructure.Data;

namespace GhostHunter.Infrastructure.Services;

public class ScrapeLogService : IScrapeLogService
{
    private readonly AppDbContext _db;

    public ScrapeLogService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ScrapeLog>> GetAllAsync(int? watchId = null, string? status = null)
    {
        var query = _db.ScrapeLogs.AsQueryable();

        if (watchId.HasValue)
            query = query.Where(l => l.WatchId == watchId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(l => l.Status == status);

        return await query.OrderByDescending(l => l.StartedAt).ToListAsync();
    }

    public async Task<ScrapeLog?> GetByIdAsync(int id)
    {
        return await _db.ScrapeLogs.FindAsync(id);
    }

    public async Task<List<ScrapeLog>> GetByWatchAsync(int watchId)
    {
        return await _db.ScrapeLogs
            .Where(l => l.WatchId == watchId)
            .OrderByDescending(l => l.StartedAt)
            .ToListAsync();
    }

    public async Task<ScrapeLog?> GetLatestByWatchAsync(int watchId)
    {
        return await _db.ScrapeLogs
            .Where(l => l.WatchId == watchId)
            .OrderByDescending(l => l.StartedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<ScrapeLog> StartAsync(int watchId)
    {
        var log = new ScrapeLog
        {
            WatchId = watchId,
            StartedAt = DateTime.UtcNow,
            Status = "running"
        };

        _db.ScrapeLogs.Add(log);
        await _db.SaveChangesAsync();
        return log;
    }

    public async Task CompleteAsync(int logId, int jobsFound, int newJobsFound)
    {
        var log = await _db.ScrapeLogs.FindAsync(logId);
        if (log == null) return;

        log.CompletedAt = DateTime.UtcNow;
        log.Status = "completed";
        log.JobsFound = jobsFound;
        log.NewJobsFound = newJobsFound;

        await _db.SaveChangesAsync();
    }

    public async Task FailAsync(int logId, string errorMessage)
    {
        var log = await _db.ScrapeLogs.FindAsync(logId);
        if (log == null) return;

        log.CompletedAt = DateTime.UtcNow;
        log.Status = "failed";
        log.ErrorMessage = errorMessage;

        await _db.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var log = await _db.ScrapeLogs.FindAsync(id);
        if (log == null) return false;

        _db.ScrapeLogs.Remove(log);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<int> DeleteByWatchAsync(int watchId)
    {
        var logs = await _db.ScrapeLogs.Where(l => l.WatchId == watchId).ToListAsync();
        _db.ScrapeLogs.RemoveRange(logs);
        await _db.SaveChangesAsync();
        return logs.Count;
    }
}
