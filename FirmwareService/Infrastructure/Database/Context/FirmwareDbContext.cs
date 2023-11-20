using FirmwareService.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FirmwareService.Infrastructure.Database.Context;

public class FirmwareDbContext : DbContext
{
    public DbSet<FileSend> FileSends { get; set; }
    
    public FirmwareDbContext(DbContextOptions<FirmwareDbContext> options) : base(options) { }
}