using GhostHunter.Core.Entities;

namespace GhostHunter.Core.Interfaces;

public interface IJobPostService
{
    Task<List<JobPost>> GetAllAsync(int? watchId = null, string? search = null);
    Task<JobPost?> GetByIdAsync(int id);
    Task<List<JobPost>> GetByWatchAsync(int watchId);
    Task<JobPost> CreateAsync(JobPost post);
    Task<List<JobPost>> CreateManyAsync(List<JobPost> posts);
    Task<bool> DeleteAsync(int id);
    Task<int> DeleteByWatchAsync(int watchId);
    Task<bool> ExistsAsync(string contentHash, int watchId);
}
