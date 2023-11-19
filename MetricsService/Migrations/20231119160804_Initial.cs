using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetricsService.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AggregatedLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Average_Value = table.Column<double>(type: "float", nullable: false),
                    Min_Value = table.Column<double>(type: "float", nullable: false),
                    Max_Value = table.Column<double>(type: "float", nullable: false),
                    Last_Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatedLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    DeviceTypeId = table.Column<int>(type: "int", nullable: false),
                    Loggable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogCollectionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogCollectionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogCollections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<int>(type: "int", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogCollectionTypeId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogCollections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogCollections_LogCollectionTypes_LogCollectionTypeId",
                        column: x => x.LogCollectionTypeId,
                        principalTable: "LogCollectionTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LogValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<float>(type: "real", nullable: true),
                    FieldId = table.Column<int>(type: "int", nullable: true),
                    CollectionId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogValues_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LogValues_LogCollections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "LogCollections",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogCollections_LogCollectionTypeId",
                table: "LogCollections",
                column: "LogCollectionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LogValues_CollectionId",
                table: "LogValues",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_LogValues_FieldId",
                table: "LogValues",
                column: "FieldId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AggregatedLogs");

            migrationBuilder.DropTable(
                name: "LogValues");

            migrationBuilder.DropTable(
                name: "Fields");

            migrationBuilder.DropTable(
                name: "LogCollections");

            migrationBuilder.DropTable(
                name: "LogCollectionTypes");
        }
    }
}
