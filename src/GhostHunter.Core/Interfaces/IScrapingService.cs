using GhostHunter.Core.DTOs;
using GhostHunter.Core.Entities;

namespace GhostHunter.Core.Interfaces;

public interface IScrapingService
{
    Task<List<ScrapeResult>> ScrapeAsync(string url, string selector);
    Task<List<ScrapeResult>> ScrapeAsync(JobWatch jobWatch);
    Task<List<ScrapeResult>> FilterByKeywordsAsync(List<ScrapeResult> results, JobWatch jobWatch);
}
