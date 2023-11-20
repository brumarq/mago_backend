using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceService.Migrations
{
    /// <inheritdoc />
    public partial class Initialv4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceTypes_TypeId",
                table: "Devices");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "Devices",
                newName: "DeviceTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Devices_TypeId",
                table: "Devices",
                newName: "IX_Devices_DeviceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_DeviceTypes_DeviceTypeId",
                table: "Devices",
                column: "DeviceTypeId",
                principalTable: "DeviceTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceTypes_DeviceTypeId",
                table: "Devices");

            migrationBuilder.RenameColumn(
                name: "DeviceTypeId",
                table: "Devices",
                newName: "TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Devices_DeviceTypeId",
                table: "Devices",
                newName: "IX_Devices_TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_DeviceTypes_TypeId",
                table: "Devices",
                column: "TypeId",
                principalTable: "DeviceTypes",
                principalColumn: "Id");
        }
    }
}
