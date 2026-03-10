using GhostHunter.Infrastructure.Data;
using GhostHunter.Core.Entities;

namespace GhostHunter.Api.GraphQL;

public class Query
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<JobWatch> GetWatches(AppDbContext context)
        => context.JobWatches;

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<JobPost> GetJobPosts(AppDbContext context)
        => context.JobPosts;

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Alert> GetAlerts(AppDbContext context)
        => context.Alerts;

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<User> GetUsers(AppDbContext context)
        => context.Users;
}