using Microsoft.EntityFrameworkCore;
using GhostHunter.Core.Entities;
using GhostHunter.Core.Interfaces;
using GhostHunter.Infrastructure.Data;

namespace GhostHunter.Infrastructure.Services;

public class WatchService : IWatchService
{
    private readonly AppDbContext _db;

    public WatchService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<JobWatch>> GetAllAsync(int? userId = null)
    {
        var query = _db.JobWatches.AsQueryable();

        if (userId.HasValue)
            query = query.Where(w => w.UserId == userId.Value);

        return await query.OrderByDescending(w => w.CreatedAt).ToListAsync();
    }

    public async Task<JobWatch?> GetByIdAsync(int id)
    {
        return await _db.JobWatches.FindAsync(id);
    }

    public async Task<List<JobWatch>> GetActiveAsync()
    {
        return await _db.JobWatches
            .Where(w => w.IsActive)
            .ToListAsync();
    }

    public async Task<JobWatch> CreateAsync(JobWatch watch)
    {
        watch.CreatedAt = DateTime.UtcNow;
        _db.JobWatches.Add(watch);
        await _db.SaveChangesAsync();
        return watch;
    }

    public async Task UpdateAsync(JobWatch watch)
    {
        _db.JobWatches.Update(watch);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var watch = await _db.JobWatches.FindAsync(id);
        if (watch == null) return false;

        _db.JobWatches.Remove(watch);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task UpdateLastCheckedAsync(int id, string? errorMessage = null)
    {
        var watch = await _db.JobWatches.FindAsync(id);
        if (watch == null) return;

        watch.LastCheckedAt = DateTime.UtcNow;

        if (errorMessage == null)
        {
            watch.ConsecutiveFailures = 0;
            watch.LastErrorMessage = null;
        }
        else
        {
            watch.ConsecutiveFailures++;
            watch.LastErrorMessage = errorMessage;
        }

        await _db.SaveChangesAsync();
    }
}
