using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace DAL;
public class CustomDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Empoloyees { get; set; }

    public CustomDbContext(DbContextOptions<CustomDbContext> options) : base(options) { }
}

