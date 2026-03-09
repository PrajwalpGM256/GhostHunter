using GhostHunter.Core.Entities;

namespace GhostHunter.Core.Interfaces;

public interface IAlertService
{
    Task<List<Alert>> GetAllAsync(int? userId = null, string? status = null);
    Task<Alert?> GetByIdAsync(int id);
    Task<List<Alert>> GetByUserAsync(int userId);
    Task<List<Alert>> GetByWatchAsync(int watchId);
    Task<Alert> CreateAsync(Alert alert);
    Task<List<Alert>> CreateForNewPostsAsync(int userId, int watchId, List<JobPost> newPosts);
    Task UpdateStatusAsync(int id, string status, string? errorMessage = null);
    Task<bool> DeleteAsync(int id);
}
