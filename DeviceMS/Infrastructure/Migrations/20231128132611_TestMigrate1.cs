using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TestMigrate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Setting_Unit_UnitId",
                table: "Setting");

            migrationBuilder.AlterColumn<int>(
                name: "UnitId",
                table: "Setting",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Setting_Unit_UnitId",
                table: "Setting",
                column: "UnitId",
                principalTable: "Unit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Setting_Unit_UnitId",
                table: "Setting");

            migrationBuilder.AlterColumn<int>(
                name: "UnitId",
                table: "Setting",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Setting_Unit_UnitId",
                table: "Setting",
                column: "UnitId",
                principalTable: "Unit",
                principalColumn: "Id");
        }
    }
}
