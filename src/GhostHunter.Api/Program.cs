using Microsoft.EntityFrameworkCore;
using GhostHunter.Infrastructure.Data;
using GhostHunter.Core.Interfaces;
using GhostHunter.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<IScrapingService, ScrapingService>();
builder.Services.AddScoped<IJobPostService, JobPostService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IScrapeLogService, ScrapeLogService>();
builder.Services.AddScoped<IWatchService, WatchService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
