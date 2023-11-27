using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TempFixUnitQuantityV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuantityUnit_Quantity_BaseOfQuantitiesId",
                table: "QuantityUnit");

            migrationBuilder.DropForeignKey(
                name: "FK_QuantityUnit_Unit_UnitsId",
                table: "QuantityUnit");

            migrationBuilder.RenameColumn(
                name: "UnitsId",
                table: "QuantityUnit",
                newName: "UnitId");

            migrationBuilder.RenameColumn(
                name: "BaseOfQuantitiesId",
                table: "QuantityUnit",
                newName: "QuantityId");

            migrationBuilder.RenameIndex(
                name: "IX_QuantityUnit_UnitsId",
                table: "QuantityUnit",
                newName: "IX_QuantityUnit_UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuantityUnit_Quantity_QuantityId",
                table: "QuantityUnit",
                column: "QuantityId",
                principalTable: "Quantity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuantityUnit_Unit_UnitId",
                table: "QuantityUnit",
                column: "UnitId",
                principalTable: "Unit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuantityUnit_Quantity_QuantityId",
                table: "QuantityUnit");

            migrationBuilder.DropForeignKey(
                name: "FK_QuantityUnit_Unit_UnitId",
                table: "QuantityUnit");

            migrationBuilder.RenameColumn(
                name: "UnitId",
                table: "QuantityUnit",
                newName: "UnitsId");

            migrationBuilder.RenameColumn(
                name: "QuantityId",
                table: "QuantityUnit",
                newName: "BaseOfQuantitiesId");

            migrationBuilder.RenameIndex(
                name: "IX_QuantityUnit_UnitId",
                table: "QuantityUnit",
                newName: "IX_QuantityUnit_UnitsId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuantityUnit_Quantity_BaseOfQuantitiesId",
                table: "QuantityUnit",
                column: "BaseOfQuantitiesId",
                principalTable: "Quantity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuantityUnit_Unit_UnitsId",
                table: "QuantityUnit",
                column: "UnitsId",
                principalTable: "Unit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
