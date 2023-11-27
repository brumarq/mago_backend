﻿// <auto-generated />
using System;
using Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(DevicesDbContext))]
    [Migration("20231127114927_RevertMigrationv1")]
    partial class RevertMigrationv1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AuthId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("DeviceTypeId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PwHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Salt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("SendSettingsAtConn")
                        .HasColumnType("bit");

                    b.Property<bool>("SendSettingsNow")
                        .HasColumnType("bit");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DeviceTypeId");

                    b.ToTable("Device");
                });

            modelBuilder.Entity("Domain.Entities.DeviceLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeviceId")
                        .HasColumnType("int");

                    b.Property<int?>("LocationId")
                        .HasColumnType("int");

                    b.Property<string>("Ownership")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("PlacedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RemovedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("LocationId");

                    b.ToTable("DeviceLocation");
                });

            modelBuilder.Entity("Domain.Entities.DeviceType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("DeviceType");
                });

            modelBuilder.Entity("Domain.Entities.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Street")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Zip")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("Domain.Entities.Quantity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("BaseUnitId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("BaseUnitId");

                    b.ToTable("Quantity");
                });

            modelBuilder.Entity("Domain.Entities.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<float?>("DefaultValue")
                        .HasColumnType("real");

                    b.Property<int?>("DeviceTypeId")
                        .HasColumnType("int");

                    b.Property<string>("EditedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UnitId")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ViewedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DeviceTypeId");

                    b.HasIndex("UnitId");

                    b.ToTable("Setting");
                });

            modelBuilder.Entity("Domain.Entities.SettingValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeviceId")
                        .HasColumnType("int");

                    b.Property<int?>("SettingId")
                        .HasColumnType("int");

                    b.Property<string>("UpdateStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<float>("Value")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("SettingId");

                    b.ToTable("SettingValue");
                });

            modelBuilder.Entity("Domain.Entities.Unit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<float?>("Factor")
                        .HasColumnType("real");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float?>("Offset")
                        .HasColumnType("real");

                    b.Property<int?>("QuantityId")
                        .HasColumnType("int");

                    b.Property<string>("Symbol")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("QuantityId");

                    b.ToTable("Unit");
                });

            modelBuilder.Entity("Domain.Entities.UsersOnDevices", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("ConnectionMail")
                        .HasColumnType("bit");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeviceId")
                        .HasColumnType("int");

                    b.Property<string>("Role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.ToTable("UsersOnDevices");
                });

            modelBuilder.Entity("Domain.Entities.Device", b =>
                {
                    b.HasOne("Domain.Entities.DeviceType", "DeviceType")
                        .WithMany()
                        .HasForeignKey("DeviceTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DeviceType");
                });

            modelBuilder.Entity("Domain.Entities.DeviceLocation", b =>
                {
                    b.HasOne("Domain.Entities.Device", "Device")
                        .WithMany()
                        .HasForeignKey("DeviceId");

                    b.HasOne("Domain.Entities.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.Navigation("Device");

                    b.Navigation("Location");
                });

            modelBuilder.Entity("Domain.Entities.Quantity", b =>
                {
                    b.HasOne("Domain.Entities.Unit", "BaseUnit")
                        .WithMany("BaseOfQuantities")
                        .HasForeignKey("BaseUnitId");

                    b.Navigation("BaseUnit");
                });

            modelBuilder.Entity("Domain.Entities.Setting", b =>
                {
                    b.HasOne("Domain.Entities.DeviceType", "DeviceType")
                        .WithMany()
                        .HasForeignKey("DeviceTypeId");

                    b.HasOne("Domain.Entities.Unit", "Unit")
                        .WithMany()
                        .HasForeignKey("UnitId");

                    b.Navigation("DeviceType");

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("Domain.Entities.SettingValue", b =>
                {
                    b.HasOne("Domain.Entities.Device", "Device")
                        .WithMany()
                        .HasForeignKey("DeviceId");

                    b.HasOne("Domain.Entities.Setting", "Setting")
                        .WithMany()
                        .HasForeignKey("SettingId");

                    b.Navigation("Device");

                    b.Navigation("Setting");
                });

            modelBuilder.Entity("Domain.Entities.Unit", b =>
                {
                    b.HasOne("Domain.Entities.Quantity", "Quantity")
                        .WithMany("Units")
                        .HasForeignKey("QuantityId");

                    b.Navigation("Quantity");
                });

            modelBuilder.Entity("Domain.Entities.UsersOnDevices", b =>
                {
                    b.HasOne("Domain.Entities.Device", "Device")
                        .WithMany()
                        .HasForeignKey("DeviceId");

                    b.Navigation("Device");
                });

            modelBuilder.Entity("Domain.Entities.Quantity", b =>
                {
                    b.Navigation("Units");
                });

            modelBuilder.Entity("Domain.Entities.Unit", b =>
                {
                    b.Navigation("BaseOfQuantities");
                });
#pragma warning restore 612, 618
        }
    }
}
