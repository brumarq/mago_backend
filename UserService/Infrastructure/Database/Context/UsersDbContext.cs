using Microsoft.EntityFrameworkCore;
using UserService.Core.Domain.Models;

namespace UserService.Infrastructure.Database.Context;

public class UsersDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }
}