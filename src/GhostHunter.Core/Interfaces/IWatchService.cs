using GhostHunter.Core.Entities;

namespace GhostHunter.Core.Interfaces;

public interface IWatchService
{
    Task<List<JobWatch>> GetAllAsync(int? userId = null);
    Task<JobWatch?> GetByIdAsync(int id);
    Task<List<JobWatch>> GetActiveAsync();
    Task<JobWatch> CreateAsync(JobWatch watch);
    Task UpdateAsync(JobWatch watch);
    Task<bool> DeleteAsync(int id);
    Task UpdateLastCheckedAsync(int id, string? errorMessage = null);
}
