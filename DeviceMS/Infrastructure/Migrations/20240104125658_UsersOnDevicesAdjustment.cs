using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UsersOnDevicesAdjustment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersOnDevices_Device_DeviceId",
                table: "UsersOnDevices");

            migrationBuilder.DropTable(
                name: "QuantityUnit");

            migrationBuilder.DropTable(
                name: "Quantity");

            migrationBuilder.DropColumn(
                name: "ConnectionMail",
                table: "UsersOnDevices");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "UsersOnDevices");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "UsersOnDevices",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersOnDevices_Device_DeviceId",
                table: "UsersOnDevices",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersOnDevices_Device_DeviceId",
                table: "UsersOnDevices");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "UsersOnDevices",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "ConnectionMail",
                table: "UsersOnDevices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "UsersOnDevices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Quantity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TestInt = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quantity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuantityUnit",
                columns: table => new
                {
                    QuantityId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuantityUnit", x => new { x.QuantityId, x.UnitId });
                    table.ForeignKey(
                        name: "FK_QuantityUnit_Quantity_QuantityId",
                        column: x => x.QuantityId,
                        principalTable: "Quantity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuantityUnit_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuantityUnit_UnitId",
                table: "QuantityUnit",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersOnDevices_Device_DeviceId",
                table: "UsersOnDevices",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id");
        }
    }
}
