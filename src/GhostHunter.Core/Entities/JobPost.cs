using System.Text.Json.Serialization;

namespace GhostHunter.Core.Entities;

public class JobPost
{
    public int Id { get; set; }
    public int WatchId { get; set; }
    
    public string Title { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? Location { get; set; }
    public string Url { get; set; } = string.Empty;
    public string ContentHash { get; set; } = string.Empty;
    public string MatchedKeywords { get; set; } = "[]";
    
    public DateTime FirstSeenAt { get; set; } = DateTime.UtcNow;
    
    [JsonIgnore]
    public JobWatch Watch { get; set; } = null!;
    [JsonIgnore]
    public List<Alert> Alerts { get; set; } = new();
}
