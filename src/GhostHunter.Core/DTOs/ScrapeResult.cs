namespace GhostHunter.Core.DTOs;

public class ScrapeResult
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public DateTime ScrapedAt { get; set; } = DateTime.UtcNow;
}
