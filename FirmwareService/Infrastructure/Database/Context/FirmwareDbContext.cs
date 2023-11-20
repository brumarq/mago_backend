using FirmwareService.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FirmwareService.Infrastructure.Database.Context;

public class FirmwareDbContext : DbContext
{
    public DbSet<FileSend> FileSends { get; set; }
    
    public FirmwareDbContext(DbContextOptions<FirmwareDbContext> options) : base(options) { }
    
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