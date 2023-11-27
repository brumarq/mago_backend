using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration;

public class SettingValueConfiguration : IEntityTypeConfiguration<SettingValue>
{
    public void Configure(EntityTypeBuilder<SettingValue> builder)
    {
        builder.Navigation(sv => sv.Setting).AutoInclude();
    }
}