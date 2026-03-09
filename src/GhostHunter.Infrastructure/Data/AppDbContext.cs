using Microsoft.EntityFrameworkCore;
using GhostHunter.Core.Entities;

namespace GhostHunter.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<JobWatch> JobWatches => Set<JobWatch>();
    public DbSet<JobPost> JobPosts => Set<JobPost>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<ScrapeLog> ScrapeLogs => Set<ScrapeLog>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
        });
        
        modelBuilder.Entity<JobWatch>(entity =>
        {
            entity.HasOne(e => e.User)
                  .WithMany(u => u.JobWatches)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<JobPost>(entity =>
        {
            entity.HasOne(e => e.Watch)
                  .WithMany(w => w.JobPosts)
                  .HasForeignKey(e => e.WatchId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.ContentHash);
        });
        
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Watch)
                  .WithMany()
                  .HasForeignKey(e => e.WatchId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.JobPost)
                  .WithMany(j => j.Alerts)
                  .HasForeignKey(e => e.JobPostId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<ScrapeLog>(entity =>
        {
            entity.HasOne(e => e.Watch)
                  .WithMany()
                  .HasForeignKey(e => e.WatchId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
