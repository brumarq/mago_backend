using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedCompanyIdKeyToSettingValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SettingValue_Device_DeviceId",
                table: "SettingValue");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "SettingValue",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SettingValue_Device_DeviceId",
                table: "SettingValue",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SettingValue_Device_DeviceId",
                table: "SettingValue");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "SettingValue",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_SettingValue_Device_DeviceId",
                table: "SettingValue",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id");
        }
    }
}
