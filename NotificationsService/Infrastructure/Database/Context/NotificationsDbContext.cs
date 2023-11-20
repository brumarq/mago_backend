using Microsoft.EntityFrameworkCore;
using NotificationsService.Core.Domain.Models;

namespace NotificationsService.Infrastructure.Database.Context;

public class NotificationsDbContext : DbContext
{
    public DbSet<Status> Statuses { get; set; }
    public DbSet<StatusType> StatusTypes { get; set; }
    public DbSet<UserOnStatusType> UserOnStatusTypes { get; set; }
    
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : base(options) { }
}