using Microsoft.EntityFrameworkCore;
using Model;
using Model.Entities.Devices;
using Model.Entities.Users;

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
    public DbSet<FileSend> FileSends { get; set; }
    public DbSet<AggregatedLog> AggregatedLogs { get; set; }
    public DbSet<Field> Fields { get; set; }
    public DbSet<LogCollection> LogCollections { get; set; }
    public DbSet<LogCollectionType> LogCollectionTypes { get; set; }
    public DbSet<LogValue> LogValues { get; set; }
    public DbSet<Status> Statusses { get; set; }
    public DbSet<StatusType> StatusTypes { get; set; }
    public DbSet<UserOnStatusType> UserOnStatusTypes { get; set; }
    public DbSet<UsersOnDevices> UsersOnDevices { get; set; }
    public DbSet<User> Users { get; set; }

    public CustomDbContext(DbContextOptions<CustomDbContext> options) : base(options) { }

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