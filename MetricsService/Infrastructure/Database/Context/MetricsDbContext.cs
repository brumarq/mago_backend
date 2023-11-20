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
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries().Where(
            e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            ((BaseEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;
            // Can also add UpdatedBy (user)

            if (entry.State == EntityState.Added)
            {
                ((BaseEntity)entry.Entity).CreatedAt = DateTime.UtcNow;
                // Can also add CreatedBy (user)
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}