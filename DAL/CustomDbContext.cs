using Microsoft.EntityFrameworkCore;
using Model.Entities.Devices;

namespace DAL;
public class CustomDbContext : DbContext
{
    public DbSet<Device> Devices { get; set; }
    public DbSet<DeviceLocation> DeviceLocations { get; set; }
    public DbSet<DeviceType> DeviceTypes { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Quantity> Quantities { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public DbSet<SettingValue> SettingValues { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<UsersOnDevices> UsersOnDevices { get; set; }

    public CustomDbContext(DbContextOptions<CustomDbContext> options) : base(options) { }
}

