using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Zip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceTypeId = table.Column<int>(type: "int", nullable: true),
                    SendSettingsAtConn = table.Column<bool>(type: "bit", nullable: false),
                    SendSettingsNow = table.Column<bool>(type: "bit", nullable: false),
                    AuthId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PwHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Device_DeviceType_DeviceTypeId",
                        column: x => x.DeviceTypeId,
                        principalTable: "DeviceType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DeviceLocation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<int>(type: "int", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    Ownership = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlacedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RemovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceLocation_Device_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Device",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DeviceLocation_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UsersOnDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DeviceId = table.Column<int>(type: "int", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConnectionMail = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersOnDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersOnDevices_Device_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Device",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Quantity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseUnitId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quantity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Unit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Factor = table.Column<float>(type: "real", nullable: true),
                    Offset = table.Column<float>(type: "real", nullable: true),
                    QuantityId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Unit_Quantity_QuantityId",
                        column: x => x.QuantityId,
                        principalTable: "Quantity",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Setting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultValue = table.Column<float>(type: "real", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    DeviceTypeId = table.Column<int>(type: "int", nullable: true),
                    ViewedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Setting_DeviceType_DeviceTypeId",
                        column: x => x.DeviceTypeId,
                        principalTable: "DeviceType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Setting_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SettingValue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<float>(type: "real", nullable: false),
                    SettingId = table.Column<int>(type: "int", nullable: true),
                    UpdateStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SettingValue_Device_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Device",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SettingValue_Setting_SettingId",
                        column: x => x.SettingId,
                        principalTable: "Setting",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Device_DeviceTypeId",
                table: "Device",
                column: "DeviceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceLocation_DeviceId",
                table: "DeviceLocation",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceLocation_LocationId",
                table: "DeviceLocation",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Quantity_BaseUnitId",
                table: "Quantity",
                column: "BaseUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Setting_DeviceTypeId",
                table: "Setting",
                column: "DeviceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Setting_UnitId",
                table: "Setting",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_SettingValue_DeviceId",
                table: "SettingValue",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_SettingValue_SettingId",
                table: "SettingValue",
                column: "SettingId");

            migrationBuilder.CreateIndex(
                name: "IX_Unit_QuantityId",
                table: "Unit",
                column: "QuantityId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersOnDevices_DeviceId",
                table: "UsersOnDevices",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quantity_Unit_BaseUnitId",
                table: "Quantity",
                column: "BaseUnitId",
                principalTable: "Unit",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quantity_Unit_BaseUnitId",
                table: "Quantity");

            migrationBuilder.DropTable(
                name: "DeviceLocation");

            migrationBuilder.DropTable(
                name: "SettingValue");

            migrationBuilder.DropTable(
                name: "UsersOnDevices");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "Setting");

            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropTable(
                name: "DeviceType");

            migrationBuilder.DropTable(
                name: "Unit");

            migrationBuilder.DropTable(
                name: "Quantity");
        }
    }
}
