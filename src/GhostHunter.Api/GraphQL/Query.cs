using GhostHunter.Infrastructure.Data;
using GhostHunter.Core.Entities;
using HotChocolate.Authorization;
using System.Security.Claims;

namespace GhostHunter.Api.GraphQL;

public class Query
{
    [Authorize]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<JobWatch> GetWatches(
        AppDbContext context,
        ClaimsPrincipal claimsPrincipal)
    {
        var userId = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return context.JobWatches.Where(w => w.UserId == userId);
    }

    [Authorize]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<JobPost> GetJobPosts(
        AppDbContext context,
        ClaimsPrincipal claimsPrincipal)
    {
        var userId = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return context.JobPosts.Where(jp => jp.Watch.UserId == userId);
    }

    [Authorize]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Alert> GetAlerts(
        AppDbContext context,
        ClaimsPrincipal claimsPrincipal)
    {
        var userId = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return context.Alerts.Where(a => a.UserId == userId);
    }
}