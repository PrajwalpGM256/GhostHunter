using AngleSharp;
using AngleSharp.Dom;
using GhostHunter.Core.DTOs;
using GhostHunter.Core.Entities;
using GhostHunter.Core.Interfaces;
using System.Text.Json;

namespace GhostHunter.Infrastructure.Services;

public class ScrapingService : IScrapingService
{
    private readonly HttpClient _httpClient;

    public ScrapingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ScrapeResult>> ScrapeAsync(string url, string selector)
    {
        var results = new List<ScrapeResult>();

        var html = await _httpClient.GetStringAsync(url);

        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(html));

        var elements = document.QuerySelectorAll(selector);

        foreach (var element in elements)
        {
            var result = new ScrapeResult
            {
                Title = element.TextContent.Trim(),
                Url = ExtractLink(element, null, url),
                ScrapedAt = DateTime.UtcNow
            };

            if (!string.IsNullOrWhiteSpace(result.Title))
            {
                results.Add(result);
            }
        }

        return results;
    }

    public async Task<List<ScrapeResult>> ScrapeAsync(JobWatch jobWatch)
    {
        var results = new List<ScrapeResult>();

        try
        {
            var html = await _httpClient.GetStringAsync(jobWatch.Url);

            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(html));

            var jobElements = document.QuerySelectorAll(jobWatch.JobTitleSelector ?? "a");

            foreach (var element in jobElements)
            {
                var scrapeResult = new ScrapeResult
                {
                    Title = element.TextContent.Trim(),
                    Url = ExtractLink(element, jobWatch.LinkSelector, jobWatch.Url),
                    Company = ExtractText(element, jobWatch.CompanySelector),
                    Location = ExtractText(element, jobWatch.LocationSelector),
                    ScrapedAt = DateTime.UtcNow
                };

                if (!string.IsNullOrWhiteSpace(scrapeResult.Title))
                {
                    results.Add(scrapeResult);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Scraping failed for {jobWatch.Url}: {ex.Message}");
        }

        return results;
    }

    public Task<List<ScrapeResult>> FilterByKeywordsAsync(List<ScrapeResult> results, JobWatch jobWatch)
    {
        var filtered = results.AsEnumerable();

        var includeKeywords = JsonSerializer.Deserialize<List<string>>(jobWatch.IncludeKeywords) ?? new();
        var excludeKeywords = JsonSerializer.Deserialize<List<string>>(jobWatch.ExcludeKeywords) ?? new();

        if (includeKeywords.Any())
        {
            var lowerKeywords = includeKeywords.Select(k => k.ToLower()).ToList();
            filtered = filtered.Where(r =>
                lowerKeywords.Any(keyword =>
                    r.Title.ToLower().Contains(keyword) ||
                    (r.Description?.ToLower().Contains(keyword) ?? false)
                )
            );
        }

        if (excludeKeywords.Any())
        {
            var lowerKeywords = excludeKeywords.Select(k => k.ToLower()).ToList();
            filtered = filtered.Where(r =>
                !lowerKeywords.Any(keyword =>
                    r.Title.ToLower().Contains(keyword) ||
                    (r.Description?.ToLower().Contains(keyword) ?? false)
                )
            );
        }

        return Task.FromResult(filtered.ToList());
    }

    private string ExtractText(IElement parent, string? selector)
    {
        if (string.IsNullOrWhiteSpace(selector))
            return string.Empty;

        var element = parent.QuerySelector(selector);
        return element?.TextContent.Trim() ?? string.Empty;
    }

    private string ExtractLink(IElement parent, string? selector, string baseUrl)
    {
        var element = string.IsNullOrWhiteSpace(selector)
            ? parent.QuerySelector("a") ?? parent as IElement
            : parent.QuerySelector(selector);

        var href = element?.GetAttribute("href") ?? string.Empty;

        if (!string.IsNullOrEmpty(href) && !href.StartsWith("http"))
        {
            var baseUri = new Uri(baseUrl);
            href = new Uri(baseUri, href).ToString();
        }

        return href;
    }
}
