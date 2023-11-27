using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TempFixUnitQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quantity_Unit_BaseUnitId",
                table: "Quantity");

            migrationBuilder.DropForeignKey(
                name: "FK_Unit_Quantity_QuantityId",
                table: "Unit");

            migrationBuilder.DropIndex(
                name: "IX_Unit_QuantityId",
                table: "Unit");

            migrationBuilder.DropIndex(
                name: "IX_Quantity_BaseUnitId",
                table: "Quantity");

            migrationBuilder.DropColumn(
                name: "QuantityId",
                table: "Unit");

            migrationBuilder.DropColumn(
                name: "BaseUnitId",
                table: "Quantity");

            migrationBuilder.CreateTable(
                name: "QuantityUnit",
                columns: table => new
                {
                    BaseOfQuantitiesId = table.Column<int>(type: "int", nullable: false),
                    UnitsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuantityUnit", x => new { x.BaseOfQuantitiesId, x.UnitsId });
                    table.ForeignKey(
                        name: "FK_QuantityUnit_Quantity_BaseOfQuantitiesId",
                        column: x => x.BaseOfQuantitiesId,
                        principalTable: "Quantity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuantityUnit_Unit_UnitsId",
                        column: x => x.UnitsId,
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuantityUnit_UnitsId",
                table: "QuantityUnit",
                column: "UnitsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuantityUnit");

            migrationBuilder.AddColumn<int>(
                name: "QuantityId",
                table: "Unit",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BaseUnitId",
                table: "Quantity",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Unit_QuantityId",
                table: "Unit",
                column: "QuantityId");

            migrationBuilder.CreateIndex(
                name: "IX_Quantity_BaseUnitId",
                table: "Quantity",
                column: "BaseUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quantity_Unit_BaseUnitId",
                table: "Quantity",
                column: "BaseUnitId",
                principalTable: "Unit",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Unit_Quantity_QuantityId",
                table: "Unit",
                column: "QuantityId",
                principalTable: "Quantity",
                principalColumn: "Id");
        }
    }
}
