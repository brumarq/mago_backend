using DeviceService.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DeviceService.Infrastructure.Database.Context;

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
        // Quantity
        modelBuilder.Entity<Quantity>(entity =>
        {
            entity.HasOne(q => q.BaseUnit)
                .WithMany(u => u.BaseOfQuantities)
                .IsRequired(false);
        });

        base.OnModelCreating(modelBuilder);
    }
}