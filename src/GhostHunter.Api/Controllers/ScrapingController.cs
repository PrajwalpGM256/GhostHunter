using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GhostHunter.Core.Entities;
using GhostHunter.Core.Interfaces;
using GhostHunter.Api.Extensions;
using System.Security.Cryptography;
using System.Text;

namespace GhostHunter.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScrapingController : ControllerBase
{
    private readonly IScrapingService _scrapingService;
    private readonly IWatchService _watchService;
    private readonly IJobPostService _jobPostService;
    private readonly IScrapeLogService _scrapeLogService;
    private readonly IAlertService _alertService;

    public ScrapingController(
        IScrapingService scrapingService,
        IWatchService watchService,
        IJobPostService jobPostService,
        IScrapeLogService scrapeLogService,
        IAlertService alertService)
    {
        _scrapingService = scrapingService;
        _watchService = watchService;
        _jobPostService = jobPostService;
        _scrapeLogService = scrapeLogService;
        _alertService = alertService;
    }

    [HttpPost("test")]
    public async Task<IActionResult> TestScrape([FromBody] TestScrapeRequest request)
    {
        var results = await _scrapingService.ScrapeAsync(request.Url, request.Selector);
        return Ok(results);
    }

    [HttpPost("watch/{id}")]
    public async Task<IActionResult> ScrapeJobWatch(int id)
    {
        var jobWatch = await _watchService.GetByIdAsync(id);
        if (jobWatch == null)
            return NotFound($"JobWatch with ID {id} not found");

        if (jobWatch.UserId != User.GetUserId())
            return Forbid();

        if (!jobWatch.IsActive)
            return BadRequest("JobWatch is not active");

        var scrapeLog = await _scrapeLogService.StartAsync(id);

        try
        {
            var results = await _scrapingService.ScrapeAsync(jobWatch);
            var filtered = await _scrapingService.FilterByKeywordsAsync(results, jobWatch);

            var newPosts = new List<JobPost>();

            foreach (var result in filtered)
            {
                var hash = ComputeHash(result.Title + result.Url);

                if (await _jobPostService.ExistsAsync(hash, id))
                    continue;

                var jobPost = new JobPost
                {
                    WatchId = id,
                    Title = result.Title,
                    Company = result.Company,
                    Location = result.Location,
                    Url = result.Url,
                    ContentHash = hash,
                    FirstSeenAt = DateTime.UtcNow
                };

                newPosts.Add(jobPost);
            }

            if (newPosts.Any())
            {
                await _jobPostService.CreateManyAsync(newPosts);
                await _alertService.CreateForNewPostsAsync(jobWatch.UserId, id, newPosts);
            }

            await _watchService.UpdateLastCheckedAsync(id);
            await _scrapeLogService.CompleteAsync(scrapeLog.Id, results.Count, newPosts.Count);

            return Ok(new
            {
                TotalScraped = results.Count,
                AfterFiltering = filtered.Count,
                NewPostsSaved = newPosts.Count,
                NewPosts = newPosts
            });
        }
        catch (Exception ex)
        {
            await _watchService.UpdateLastCheckedAsync(id, ex.Message);
            await _scrapeLogService.FailAsync(scrapeLog.Id, ex.Message);

            return StatusCode(500, new { Error = ex.Message });
        }
    }

    private static string ComputeHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}

public class TestScrapeRequest
{
    public string Url { get; set; } = string.Empty;
    public string Selector { get; set; } = string.Empty;
}
