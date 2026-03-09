using System.Text.Json.Serialization;

namespace GhostHunter.Core.Entities;

public class Alert
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int WatchId { get; set; }
    public int JobPostId { get; set; }
    
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public string Channel { get; set; } = "email";
    public string Status { get; set; } = "pending";
    public string? ErrorMessage { get; set; }
    
    [JsonIgnore]
    public User User { get; set; } = null!;
    [JsonIgnore]
    public JobWatch Watch { get; set; } = null!;
    [JsonIgnore]
    public JobPost JobPost { get; set; } = null!;
}
