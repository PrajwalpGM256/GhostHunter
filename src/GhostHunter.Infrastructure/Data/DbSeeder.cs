using GhostHunter.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GhostHunter.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var user = new User
        {
            Email = "demo@ghosthunter.com",
            PasswordHash = "temp-hash",
            EmailNotifications = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var watches = new List<JobWatch>
        {
            new JobWatch
            {
                UserId = user.Id,
                Name = "Hacker News",
                Url = "https://news.ycombinator.com",
                JobTitleSelector = ".titleline > a",
                IsActive = true,
                CheckIntervalMinutes = 60,
                CreatedAt = DateTime.UtcNow
            },
            new JobWatch
            {
                UserId = user.Id,
                Name = "GitHub Trending",
                Url = "https://github.com/trending",
                JobTitleSelector = "h2.h3 a",
                IsActive = true,
                CheckIntervalMinutes = 120,
                IncludeKeywords = "[\"rust\", \"python\"]",
                CreatedAt = DateTime.UtcNow
            }
        };

        context.JobWatches.AddRange(watches);
        await context.SaveChangesAsync();

        var jobPosts = new List<JobPost>
        {
            new JobPost
            {
                WatchId = watches[0].Id,
                Title = "Show HN: I built a job scraper",
                Url = "https://news.ycombinator.com/item?id=11111",
                ContentHash = "HASH001",
                FirstSeenAt = DateTime.UtcNow.AddHours(-3)
            },
            new JobPost
            {
                WatchId = watches[0].Id,
                Title = "Ask HN: Best way to learn Rust?",
                Url = "https://news.ycombinator.com/item?id=22222",
                ContentHash = "HASH002",
                FirstSeenAt = DateTime.UtcNow.AddHours(-2)
            },
            new JobPost
            {
                WatchId = watches[0].Id,
                Title = "Launch HN: AI-powered code review tool",
                Url = "https://news.ycombinator.com/item?id=33333",
                ContentHash = "HASH003",
                FirstSeenAt = DateTime.UtcNow.AddHours(-1)
            },
            new JobPost
            {
                WatchId = watches[1].Id,
                Title = "awesome-python",
                Company = "vinta",
                Url = "https://github.com/vinta/awesome-python",
                ContentHash = "HASH004",
                MatchedKeywords = "[\"python\"]",
                FirstSeenAt = DateTime.UtcNow.AddMinutes(-45)
            },
            new JobPost
            {
                WatchId = watches[1].Id,
                Title = "rustlings",
                Company = "rust-lang",
                Url = "https://github.com/rust-lang/rustlings",
                ContentHash = "HASH005",
                MatchedKeywords = "[\"rust\"]",
                FirstSeenAt = DateTime.UtcNow.AddMinutes(-30)
            }
        };

        context.JobPosts.AddRange(jobPosts);
        await context.SaveChangesAsync();

        var alerts = new List<Alert>
        {
            new Alert
            {
                UserId = user.Id,
                WatchId = watches[0].Id,
                JobPostId = jobPosts[0].Id,
                Channel = "email",
                Status = "sent",
                SentAt = DateTime.UtcNow.AddHours(-3)
            },
            new Alert
            {
                UserId = user.Id,
                WatchId = watches[0].Id,
                JobPostId = jobPosts[1].Id,
                Channel = "email",
                Status = "sent",
                SentAt = DateTime.UtcNow.AddHours(-2)
            },
            new Alert
            {
                UserId = user.Id,
                WatchId = watches[0].Id,
                JobPostId = jobPosts[2].Id,
                Channel = "email",
                Status = "pending",
                SentAt = DateTime.UtcNow.AddHours(-1)
            },
            new Alert
            {
                UserId = user.Id,
                WatchId = watches[1].Id,
                JobPostId = jobPosts[3].Id,
                Channel = "email",
                Status = "failed",
                ErrorMessage = "SMTP timeout",
                SentAt = DateTime.UtcNow.AddMinutes(-45)
            },
            new Alert
            {
                UserId = user.Id,
                WatchId = watches[1].Id,
                JobPostId = jobPosts[4].Id,
                Channel = "email",
                Status = "pending",
                SentAt = DateTime.UtcNow.AddMinutes(-30)
            }
        };

        context.Alerts.AddRange(alerts);
        await context.SaveChangesAsync();

        var scrapeLogs = new List<ScrapeLog>
        {
            new ScrapeLog
            {
                WatchId = watches[0].Id,
                StartedAt = DateTime.UtcNow.AddHours(-3),
                CompletedAt = DateTime.UtcNow.AddHours(-3).AddSeconds(4),
                Status = "completed",
                JobsFound = 30,
                NewJobsFound = 3
            },
            new ScrapeLog
            {
                WatchId = watches[0].Id,
                StartedAt = DateTime.UtcNow.AddHours(-2),
                CompletedAt = DateTime.UtcNow.AddHours(-2).AddSeconds(3),
                Status = "completed",
                JobsFound = 30,
                NewJobsFound = 0
            },
            new ScrapeLog
            {
                WatchId = watches[1].Id,
                StartedAt = DateTime.UtcNow.AddMinutes(-45),
                CompletedAt = DateTime.UtcNow.AddMinutes(-45).AddSeconds(6),
                Status = "completed",
                JobsFound = 25,
                NewJobsFound = 2
            },
            new ScrapeLog
            {
                WatchId = watches[1].Id,
                StartedAt = DateTime.UtcNow.AddMinutes(-15),
                CompletedAt = DateTime.UtcNow.AddMinutes(-15).AddSeconds(2),
                Status = "failed",
                ErrorMessage = "Rate limited by GitHub"
            }
        };

        context.ScrapeLogs.AddRange(scrapeLogs);
        await context.SaveChangesAsync();

        Console.WriteLine("Database seeded successfully!");
        Console.WriteLine($"  - User: {user.Email}");
        Console.WriteLine($"  - Watches: {watches.Count}");
        Console.WriteLine($"  - JobPosts: {jobPosts.Count}");
        Console.WriteLine($"  - Alerts: {alerts.Count}");
        Console.WriteLine($"  - ScrapeLogs: {scrapeLogs.Count}");
    }

    public static async Task ResetAndSeedAsync(AppDbContext context)
    {
        await context.ScrapeLogs.ExecuteDeleteAsync();
        await context.Alerts.ExecuteDeleteAsync();
        await context.JobPosts.ExecuteDeleteAsync();
        await context.JobWatches.ExecuteDeleteAsync();
        await context.Users.ExecuteDeleteAsync();

        await context.Database.ExecuteSqlRawAsync("ALTER SEQUENCE \"Users_Id_seq\" RESTART WITH 1");
        await context.Database.ExecuteSqlRawAsync("ALTER SEQUENCE \"JobWatches_Id_seq\" RESTART WITH 1");
        await context.Database.ExecuteSqlRawAsync("ALTER SEQUENCE \"JobPosts_Id_seq\" RESTART WITH 1");
        await context.Database.ExecuteSqlRawAsync("ALTER SEQUENCE \"Alerts_Id_seq\" RESTART WITH 1");
        await context.Database.ExecuteSqlRawAsync("ALTER SEQUENCE \"ScrapeLogs_Id_seq\" RESTART WITH 1");

        await SeedAsync(context);
    }
}
