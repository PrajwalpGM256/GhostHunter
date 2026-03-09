using System.Text.Json.Serialization;

namespace GhostHunter.Core.Entities;

public class JobWatch
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    
    public string? JobTitleSelector { get; set; }
    public string? CompanySelector { get; set; }
    public string? LinkSelector { get; set; }
    public string? LocationSelector { get; set; }
    
    public string IncludeKeywords { get; set; } = "[]";
    public string ExcludeKeywords { get; set; } = "[]";
    
    public int CheckIntervalMinutes { get; set; } = 60;
    public bool IsActive { get; set; } = true;
    public DateTime? LastCheckedAt { get; set; }
    
    public string? LastErrorMessage { get; set; }
    public int ConsecutiveFailures { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [JsonIgnore]
    public User User { get; set; } = null!;
    [JsonIgnore]
    public List<JobPost> JobPosts { get; set; } = new();
}
