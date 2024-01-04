using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration;

public class UsersOnDevicesConfiguration : IEntityTypeConfiguration<UsersOnDevices>
{
    public void Configure(EntityTypeBuilder<UsersOnDevices> builder)
    {
        builder.Navigation(uod => uod.Device).AutoInclude();
    }
}