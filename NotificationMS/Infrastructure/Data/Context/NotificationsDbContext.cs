using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Context;

public class NotificationsDbContext : DbContext
{
    public DbSet<Status> Statuses { get; set; }
    public DbSet<StatusType> StatusTypes { get; set; }
    public DbSet<UserOnStatusType> UserOnStatusTypes { get; set; }
    
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : base(options) { }
    
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