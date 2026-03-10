using GhostHunter.Infrastructure.Data;
using GhostHunter.Core.Entities;

namespace GhostHunter.Api.GraphQL;

public class Mutation
{
    public async Task<JobWatch> CreateWatch(
        string name,
        string url,
        string? jobTitleSelector,
        string? companySelector,
        string? linkSelector,
        string? locationSelector,
        int checkIntervalMinutes,
        int userId,
        AppDbContext context)
    {
        var watch = new JobWatch
        {
            Name = name,
            Url = url,
            JobTitleSelector = jobTitleSelector,
            CompanySelector = companySelector,
            LinkSelector = linkSelector,
            LocationSelector = locationSelector,
            CheckIntervalMinutes = checkIntervalMinutes,
            UserId = userId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.JobWatches.Add(watch);
        await context.SaveChangesAsync();
        return watch;
    }

    public async Task<JobWatch?> UpdateWatch(
        int id,
        string? name,
        string? url,
        bool? isActive,
        int? checkIntervalMinutes,
        AppDbContext context)
    {
        var watch = await context.JobWatches.FindAsync(id);
        if (watch == null) return null;

        if (name != null) watch.Name = name;
        if (url != null) watch.Url = url;
        if (isActive.HasValue) watch.IsActive = isActive.Value;
        if (checkIntervalMinutes.HasValue) watch.CheckIntervalMinutes = checkIntervalMinutes.Value;

        await context.SaveChangesAsync();
        return watch;
    }

    public async Task<bool> DeleteWatch(int id, AppDbContext context)
    {
        var watch = await context.JobWatches.FindAsync(id);
        if (watch == null) return false;

        context.JobWatches.Remove(watch);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Alert?> UpdateAlertStatus(int id, string status, AppDbContext context)
    {
        var alert = await context.Alerts.FindAsync(id);
        if (alert == null) return null;

        alert.Status = status;
        await context.SaveChangesAsync();
        return alert;
    }

    public async Task<bool> DeleteJobPost(int id, AppDbContext context)
    {
        var jobPost = await context.JobPosts.FindAsync(id);
        if (jobPost == null) return false;

        context.JobPosts.Remove(jobPost);
        await context.SaveChangesAsync();
        return true;
    }
}