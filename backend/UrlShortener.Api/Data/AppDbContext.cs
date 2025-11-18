using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) {}

    public DbSet<UrlEntry> Urls => Set<UrlEntry>();
    public DbSet<Click> Clicks => Set<Click>();
    public DbSet<RateLimitEntry> RateLimits => Set<RateLimitEntry>();
}
