using Microsoft.EntityFrameworkCore;
using GhostHunter.Core.Entities;
using GhostHunter.Core.Interfaces;
using GhostHunter.Infrastructure.Data;

namespace GhostHunter.Infrastructure.Services;

public class JobPostService : IJobPostService
{
    private readonly AppDbContext _db;

    public JobPostService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<JobPost>> GetAllAsync(int? watchId = null, string? search = null)
    {
        var query = _db.JobPosts.AsQueryable();

        if (watchId.HasValue)
            query = query.Where(p => p.WatchId == watchId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Title.ToLower().Contains(search.ToLower()) ||
                                     (p.Company != null && p.Company.ToLower().Contains(search.ToLower())));

        return await query.OrderByDescending(p => p.FirstSeenAt).ToListAsync();
    }

    public async Task<JobPost?> GetByIdAsync(int id)
    {
        return await _db.JobPosts.FindAsync(id);
    }

    public async Task<List<JobPost>> GetByWatchAsync(int watchId)
    {
        return await _db.JobPosts
            .Where(p => p.WatchId == watchId)
            .OrderByDescending(p => p.FirstSeenAt)
            .ToListAsync();
    }

    public async Task<JobPost> CreateAsync(JobPost post)
    {
        _db.JobPosts.Add(post);
        await _db.SaveChangesAsync();
        return post;
    }

    public async Task<List<JobPost>> CreateManyAsync(List<JobPost> posts)
    {
        if (!posts.Any()) return posts;

        _db.JobPosts.AddRange(posts);
        await _db.SaveChangesAsync();
        return posts;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var post = await _db.JobPosts.FindAsync(id);
        if (post == null) return false;

        _db.JobPosts.Remove(post);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<int> DeleteByWatchAsync(int watchId)
    {
        var posts = await _db.JobPosts.Where(p => p.WatchId == watchId).ToListAsync();
        _db.JobPosts.RemoveRange(posts);
        await _db.SaveChangesAsync();
        return posts.Count;
    }

    public async Task<bool> ExistsAsync(string contentHash, int watchId)
    {
        return await _db.JobPosts.AnyAsync(p => p.ContentHash == contentHash && p.WatchId == watchId);
    }
}
