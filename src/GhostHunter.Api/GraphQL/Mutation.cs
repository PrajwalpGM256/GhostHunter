using GhostHunter.Infrastructure.Data;
using GhostHunter.Core.Entities;
using HotChocolate.Authorization;
using System.Security.Claims;

namespace GhostHunter.Api.GraphQL;

public class Mutation
{
    [Authorize]
    public async Task<JobWatch> CreateWatch(
        string name,
        string url,
        string? jobTitleSelector,
        string? companySelector,
        string? linkSelector,
        string? locationSelector,
        int checkIntervalMinutes,
        ClaimsPrincipal claimsPrincipal,
        AppDbContext context)
    {
        var userId = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
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

    [Authorize]
    public async Task<JobWatch?> UpdateWatch(
        int id,
        string? name,
        string? url,
        bool? isActive,
        int? checkIntervalMinutes,
        ClaimsPrincipal claimsPrincipal,
        AppDbContext context)
    {
        var userId = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var watch = await context.JobWatches.FindAsync(id);
        
        if (watch == null || watch.UserId != userId) return null;

        if (name != null) watch.Name = name;
        if (url != null) watch.Url = url;
        if (isActive.HasValue) watch.IsActive = isActive.Value;
        if (checkIntervalMinutes.HasValue) watch.CheckIntervalMinutes = checkIntervalMinutes.Value;

        await context.SaveChangesAsync();
        return watch;
    }

    [Authorize]
    public async Task<bool> DeleteWatch(
        int id,
        ClaimsPrincipal claimsPrincipal,
        AppDbContext context)
    {
        var userId = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var watch = await context.JobWatches.FindAsync(id);
        
        if (watch == null || watch.UserId != userId) return false;

        context.JobWatches.Remove(watch);
        await context.SaveChangesAsync();
        return true;
    }

    [Authorize]
    public async Task<Alert?> UpdateAlertStatus(
        int id,
        string status,
        ClaimsPrincipal claimsPrincipal,
        AppDbContext context)
    {
        var userId = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var alert = await context.Alerts.FindAsync(id);
        
        if (alert == null || alert.UserId != userId) return null;

        alert.Status = status;
        await context.SaveChangesAsync();
        return alert;
    }

    [Authorize]
    public async Task<bool> DeleteJobPost(
        int id,
        ClaimsPrincipal claimsPrincipal,
        AppDbContext context)
    {
        var userId = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var jobPost = await context.JobPosts.FindAsync(id);
        
        if (jobPost == null || jobPost.Watch.UserId != userId) return false;

        context.JobPosts.Remove(jobPost);
        await context.SaveChangesAsync();
        return true;
    }
}