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
    [DbContext(typeof(MetricsDbContext))]
    [Migration("20231128133024_TestMigrate3")]
    partial class TestMigrate3
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.AggregatedLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("AverageValue")
                        .HasColumnType("float");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<double>("MaxValue")
                        .HasColumnType("float");

                    b.Property<double>("MinValue")
                        .HasColumnType("float");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("AggregatedLogs");
                });

            modelBuilder.Entity("Domain.Entities.Field", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("DeviceTypeId")
                        .HasColumnType("int");

                    b.Property<bool>("Loggable")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UnitId")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Fields");
                });

            modelBuilder.Entity("Domain.Entities.LogCollection", b =>
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

                    b.HasIndex("LogCollectionTypeId");

                    b.ToTable("LogCollections");
                });

            modelBuilder.Entity("Domain.Entities.LogCollectionType", b =>
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

            modelBuilder.Entity("Domain.Entities.LogValue", b =>
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

            modelBuilder.Entity("Domain.Entities.LogCollection", b =>
                {
                    b.HasOne("Domain.Entities.LogCollectionType", "LogCollectionType")
                        .WithMany("Collections")
                        .HasForeignKey("LogCollectionTypeId");

                    b.Navigation("LogCollectionType");
                });

            modelBuilder.Entity("Domain.Entities.LogValue", b =>
                {
                    b.HasOne("Domain.Entities.LogCollection", "Collection")
                        .WithMany("Values")
                        .HasForeignKey("CollectionId");

                    b.HasOne("Domain.Entities.Field", "Field")
                        .WithMany("LogValues")
                        .HasForeignKey("FieldId");

                    b.Navigation("Collection");

                    b.Navigation("Field");
                });

            modelBuilder.Entity("Domain.Entities.Field", b =>
                {
                    b.Navigation("LogValues");
                });

            modelBuilder.Entity("Domain.Entities.LogCollection", b =>
                {
                    b.Navigation("Values");
                });

            modelBuilder.Entity("Domain.Entities.LogCollectionType", b =>
                {
                    b.Navigation("Collections");
                });
#pragma warning restore 612, 618
        }
    }
}
