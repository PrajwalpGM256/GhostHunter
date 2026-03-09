using System.Text.Json.Serialization;

namespace GhostHunter.Core.Entities;

public class ScrapeLog
{
    public int Id { get; set; }
    public int WatchId { get; set; }
    
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string Status { get; set; } = "running";
    public int JobsFound { get; set; } = 0;
    public int NewJobsFound { get; set; } = 0;
    public string? ErrorMessage { get; set; }
    
    [JsonIgnore]
    public JobWatch Watch { get; set; } = null!;
}
