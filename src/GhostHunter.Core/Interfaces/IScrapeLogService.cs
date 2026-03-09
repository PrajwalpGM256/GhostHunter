using GhostHunter.Core.Entities;

namespace GhostHunter.Core.Interfaces;

public interface IScrapeLogService
{
    Task<List<ScrapeLog>> GetAllAsync(int? watchId = null, string? status = null);
    Task<ScrapeLog?> GetByIdAsync(int id);
    Task<List<ScrapeLog>> GetByWatchAsync(int watchId);
    Task<ScrapeLog?> GetLatestByWatchAsync(int watchId);
    Task<ScrapeLog> StartAsync(int watchId);
    Task CompleteAsync(int logId, int jobsFound, int newJobsFound);
    Task FailAsync(int logId, string errorMessage);
    Task<bool> DeleteAsync(int id);
    Task<int> DeleteByWatchAsync(int watchId);
}
