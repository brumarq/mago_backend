using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceService.Migrations
{
    /// <inheritdoc />
    public partial class Initialv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quantities_Units_UnitId",
                table: "Quantities");

            migrationBuilder.DropForeignKey(
                name: "FK_Quantities_Units_UnitId1",
                table: "Quantities");

            migrationBuilder.DropIndex(
                name: "IX_Quantities_UnitId",
                table: "Quantities");

            migrationBuilder.DropIndex(
                name: "IX_Quantities_UnitId1",
                table: "Quantities");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "Quantities");

            migrationBuilder.RenameColumn(
                name: "UnitId1",
                table: "Quantities",
                newName: "BaseUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Quantities_BaseUnitId",
                table: "Quantities",
                column: "BaseUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quantities_Units_BaseUnitId",
                table: "Quantities",
                column: "BaseUnitId",
                principalTable: "Units",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quantities_Units_BaseUnitId",
                table: "Quantities");

            migrationBuilder.DropIndex(
                name: "IX_Quantities_BaseUnitId",
                table: "Quantities");

            migrationBuilder.RenameColumn(
                name: "BaseUnitId",
                table: "Quantities",
                newName: "UnitId1");

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "Quantities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Quantities_UnitId",
                table: "Quantities",
                column: "UnitId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quantities_UnitId1",
                table: "Quantities",
                column: "UnitId1",
                unique: true,
                filter: "[UnitId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Quantities_Units_UnitId",
                table: "Quantities",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quantities_Units_UnitId1",
                table: "Quantities",
                column: "UnitId1",
                principalTable: "Units",
                principalColumn: "Id");
        }
    }
}
