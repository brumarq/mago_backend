using MetricsService.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MetricsService.Infrastructure.Database.Context;

public class MetricsDbContext : DbContext
{
    public DbSet<Field> Fields { get; set; }
    public DbSet<LogCollection> LogCollections { get; set; }
    public DbSet<LogCollectionType> LogCollectionTypes  { get; set; }
    public DbSet<LogValue> LogValues { get; set; }
    public DbSet<AggregatedLog> AggregatedLogs { get; set; }
    
    public MetricsDbContext(DbContextOptions<MetricsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // AggregatedLog
        modelBuilder.Entity<AggregatedLog>(entity =>
        {
            entity.Property(e => e.Date)
                .HasConversion(
                    d => d.ToDateTime(TimeOnly.MinValue),
                    d => DateOnly.FromDateTime(d)
                );
        });
        
        base.OnModelCreating(modelBuilder);
    }
}