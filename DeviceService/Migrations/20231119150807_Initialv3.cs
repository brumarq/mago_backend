using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceService.Migrations
{
    /// <inheritdoc />
    public partial class Initialv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuantityId",
                table: "Units",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Units_QuantityId",
                table: "Units",
                column: "QuantityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Quantities_QuantityId",
                table: "Units",
                column: "QuantityId",
                principalTable: "Quantities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_Quantities_QuantityId",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Units_QuantityId",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "QuantityId",
                table: "Units");
        }
    }
}
