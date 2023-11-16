﻿// <auto-generated />
using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(CustomDbContext))]
    [Migration("20231116192645_ModelCreationV2")]
    partial class ModelCreationV2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Model.AggregatedLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("Average_Value")
                        .HasColumnType("float");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Last_Updated")
                        .HasColumnType("datetime2");

                    b.Property<double>("Max_Value")
                        .HasColumnType("float");

                    b.Property<double>("Min_Value")
                        .HasColumnType("float");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("AggregatedLogs");
                });

            modelBuilder.Entity("Model.Entities.Devices.Device", b =>
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

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("Model.Entities.Devices.DeviceLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeviceId")
                        .HasColumnType("int");

                    b.Property<int>("LocationId")
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

                    b.ToTable("DeviceLocations");
                });

            modelBuilder.Entity("Model.Entities.Devices.DeviceType", b =>
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

                    b.ToTable("DeviceTypes");
                });

            modelBuilder.Entity("Model.Entities.Devices.Location", b =>
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

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("Model.Entities.Devices.Quantity", b =>
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

                    b.ToTable("Quantities");
                });

            modelBuilder.Entity("Model.Entities.Devices.Setting", b =>
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

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("Model.Entities.Devices.SettingValue", b =>
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

                    b.HasIndex("UserId");

                    b.ToTable("SettingValues");
                });

            modelBuilder.Entity("Model.Entities.Devices.Unit", b =>
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

                    b.ToTable("Units");
                });

            modelBuilder.Entity("Model.Entities.Devices.UsersOnDevices", b =>
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

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("UserId");

                    b.ToTable("UsersOnDevices");
                });

            modelBuilder.Entity("Model.Entities.Users.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PwHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Salt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("SysAdmin")
                        .HasColumnType("bit");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Model.Field", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeviceTypeId")
                        .HasColumnType("int");

                    b.Property<bool>("Loggable")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UnitId")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DeviceTypeId");

                    b.HasIndex("UnitId");

                    b.ToTable("Fields");
                });

            modelBuilder.Entity("Model.FileSend", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CurrPart")
                        .HasColumnType("int");

                    b.Property<int?>("DeviceId")
                        .HasColumnType("int");

                    b.Property<string>("File")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TotParts")
                        .HasColumnType("int");

                    b.Property<string>("UpdateStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("UserId");

                    b.ToTable("FileSends");
                });

            modelBuilder.Entity("Model.LogCollection", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeviceId")
                        .HasColumnType("int");

                    b.Property<int?>("LogCollectionTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("LogCollectionTypeId");

                    b.ToTable("LogCollections");
                });

            modelBuilder.Entity("Model.LogCollectionType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("LogCollectionTypes");
                });

            modelBuilder.Entity("Model.LogValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CollectionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("FieldId")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<float?>("Value")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("CollectionId");

                    b.HasIndex("FieldId");

                    b.ToTable("LogValues");
                });

            modelBuilder.Entity("Model.Status", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeviceId")
                        .HasColumnType("int");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("StatusTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("StatusTypeId");

                    b.ToTable("Statusses");
                });

            modelBuilder.Entity("Model.StatusType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("StatusTypes");
                });

            modelBuilder.Entity("Model.UserOnStatusType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeviceId")
                        .HasColumnType("int");

                    b.Property<int?>("StatusTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("StatusTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("UserOnStatusTypes");
                });

            modelBuilder.Entity("Model.Entities.Devices.Device", b =>
                {
                    b.HasOne("Model.Entities.Devices.DeviceType", "Type")
                        .WithMany("Devices")
                        .HasForeignKey("DeviceTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Type");
                });

            modelBuilder.Entity("Model.Entities.Devices.DeviceLocation", b =>
                {
                    b.HasOne("Model.Entities.Devices.Device", "Device")
                        .WithMany("DeviceLocations")
                        .HasForeignKey("DeviceId");

                    b.HasOne("Model.Entities.Devices.Location", "Location")
                        .WithMany("DeviceLocations")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Device");

                    b.Navigation("Location");
                });

            modelBuilder.Entity("Model.Entities.Devices.Quantity", b =>
                {
                    b.HasOne("Model.Entities.Devices.Unit", "BaseUnit")
                        .WithMany("BaseOfQuantities")
                        .HasForeignKey("BaseUnitId");

                    b.Navigation("BaseUnit");
                });

            modelBuilder.Entity("Model.Entities.Devices.Setting", b =>
                {
                    b.HasOne("Model.Entities.Devices.DeviceType", "DeviceType")
                        .WithMany("Settings")
                        .HasForeignKey("DeviceTypeId");

                    b.HasOne("Model.Entities.Devices.Unit", "Unit")
                        .WithMany("Settings")
                        .HasForeignKey("UnitId");

                    b.Navigation("DeviceType");

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("Model.Entities.Devices.SettingValue", b =>
                {
                    b.HasOne("Model.Entities.Devices.Device", "Device")
                        .WithMany("SettingValues")
                        .HasForeignKey("DeviceId");

                    b.HasOne("Model.Entities.Devices.Setting", "Setting")
                        .WithMany("Values")
                        .HasForeignKey("SettingId");

                    b.HasOne("Model.Entities.Users.User", "User")
                        .WithMany("SettingValues")
                        .HasForeignKey("UserId");

                    b.Navigation("Device");

                    b.Navigation("Setting");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Model.Entities.Devices.Unit", b =>
                {
                    b.HasOne("Model.Entities.Devices.Quantity", "Quantity")
                        .WithMany("Units")
                        .HasForeignKey("QuantityId");

                    b.Navigation("Quantity");
                });

            modelBuilder.Entity("Model.Entities.Devices.UsersOnDevices", b =>
                {
                    b.HasOne("Model.Entities.Devices.Device", "Device")
                        .WithMany("Users")
                        .HasForeignKey("DeviceId");

                    b.HasOne("Model.Entities.Users.User", "User")
                        .WithMany("Devices")
                        .HasForeignKey("UserId");

                    b.Navigation("Device");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Model.Field", b =>
                {
                    b.HasOne("Model.Entities.Devices.DeviceType", "DeviceType")
                        .WithMany("Fields")
                        .HasForeignKey("DeviceTypeId");

                    b.HasOne("Model.Entities.Devices.Unit", "Unit")
                        .WithMany("Fields")
                        .HasForeignKey("UnitId");

                    b.Navigation("DeviceType");

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("Model.FileSend", b =>
                {
                    b.HasOne("Model.Entities.Devices.Device", "Device")
                        .WithMany("FileSends")
                        .HasForeignKey("DeviceId");

                    b.HasOne("Model.Entities.Users.User", "User")
                        .WithMany("FileSends")
                        .HasForeignKey("UserId");

                    b.Navigation("Device");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Model.LogCollection", b =>
                {
                    b.HasOne("Model.Entities.Devices.Device", "Device")
                        .WithMany("LogCollections")
                        .HasForeignKey("DeviceId");

                    b.HasOne("Model.LogCollectionType", "LogCollectionType")
                        .WithMany("Collections")
                        .HasForeignKey("LogCollectionTypeId");

                    b.Navigation("Device");

                    b.Navigation("LogCollectionType");
                });

            modelBuilder.Entity("Model.LogValue", b =>
                {
                    b.HasOne("Model.LogCollection", "Collection")
                        .WithMany("Values")
                        .HasForeignKey("CollectionId");

                    b.HasOne("Model.Field", "Field")
                        .WithMany("LogValues")
                        .HasForeignKey("FieldId");

                    b.Navigation("Collection");

                    b.Navigation("Field");
                });

            modelBuilder.Entity("Model.Status", b =>
                {
                    b.HasOne("Model.Entities.Devices.Device", "Device")
                        .WithMany("Statusses")
                        .HasForeignKey("DeviceId");

                    b.HasOne("Model.StatusType", "StatusType")
                        .WithMany("Statusses")
                        .HasForeignKey("StatusTypeId");

                    b.Navigation("Device");

                    b.Navigation("StatusType");
                });

            modelBuilder.Entity("Model.UserOnStatusType", b =>
                {
                    b.HasOne("Model.Entities.Devices.Device", "Device")
                        .WithMany("UserOnStatusTypes")
                        .HasForeignKey("DeviceId");

                    b.HasOne("Model.StatusType", "StatusType")
                        .WithMany("UserOnStatusTypes")
                        .HasForeignKey("StatusTypeId");

                    b.HasOne("Model.Entities.Users.User", "User")
                        .WithMany("UserOnStatusTypes")
                        .HasForeignKey("UserId");

                    b.Navigation("Device");

                    b.Navigation("StatusType");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Model.Entities.Devices.Device", b =>
                {
                    b.Navigation("DeviceLocations");

                    b.Navigation("FileSends");

                    b.Navigation("LogCollections");

                    b.Navigation("SettingValues");

                    b.Navigation("Statusses");

                    b.Navigation("UserOnStatusTypes");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Model.Entities.Devices.DeviceType", b =>
                {
                    b.Navigation("Devices");

                    b.Navigation("Fields");

                    b.Navigation("Settings");
                });

            modelBuilder.Entity("Model.Entities.Devices.Location", b =>
                {
                    b.Navigation("DeviceLocations");
                });

            modelBuilder.Entity("Model.Entities.Devices.Quantity", b =>
                {
                    b.Navigation("Units");
                });

            modelBuilder.Entity("Model.Entities.Devices.Setting", b =>
                {
                    b.Navigation("Values");
                });

            modelBuilder.Entity("Model.Entities.Devices.Unit", b =>
                {
                    b.Navigation("BaseOfQuantities");

                    b.Navigation("Fields");

                    b.Navigation("Settings");
                });

            modelBuilder.Entity("Model.Entities.Users.User", b =>
                {
                    b.Navigation("Devices");

                    b.Navigation("FileSends");

                    b.Navigation("SettingValues");

                    b.Navigation("UserOnStatusTypes");
                });

            modelBuilder.Entity("Model.Field", b =>
                {
                    b.Navigation("LogValues");
                });

            modelBuilder.Entity("Model.LogCollection", b =>
                {
                    b.Navigation("Values");
                });

            modelBuilder.Entity("Model.LogCollectionType", b =>
                {
                    b.Navigation("Collections");
                });

            modelBuilder.Entity("Model.StatusType", b =>
                {
                    b.Navigation("Statusses");

                    b.Navigation("UserOnStatusTypes");
                });
#pragma warning restore 612, 618
        }
    }
}
