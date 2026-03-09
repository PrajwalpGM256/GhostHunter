using System.Text.Json.Serialization;

namespace GhostHunter.Core.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? TelegramChatId { get; set; }
    public bool EmailNotifications { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [JsonIgnore]
    public List<JobWatch> JobWatches { get; set; } = new();
}
