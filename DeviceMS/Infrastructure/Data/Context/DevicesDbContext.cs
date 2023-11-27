using System.Reflection;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Context;

public class DevicesDbContext : DbContext
{
    public DbSet<Device> Device { get; set; }
    public DbSet<DeviceType> DeviceType { get; set; }
    public DbSet<UsersOnDevices> UsersOnDevices { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<DeviceLocation> DeviceLocation { get; set; }
    public DbSet<SettingValue> SettingValue { get; set; }
    public DbSet<Setting> Setting { get; set; }
    public DbSet<Unit> Unit { get; set; }
    public DbSet<Quantity> Quantity { get; set; }

    public DevicesDbContext(DbContextOptions<DevicesDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries().Where(
            e => e.Entity is BaseEntity && e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            var baseEntity = (BaseEntity)entry.Entity;
            baseEntity.UpdatedAt = DateTime.UtcNow;
            // Can also add UpdatedBy (user)

            if (entry.State != EntityState.Added) continue;
            
            baseEntity.CreatedAt = DateTime.UtcNow;
            // Can also add CreatedBy (user)
                
            if (entry.Entity is SettingValue settingValue)
                settingValue.UpdateStatus = "New";
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}