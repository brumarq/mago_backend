using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceService.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHelloBrunoTestv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HelloBruno",
                table: "Device");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HelloBruno",
                table: "Device",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
