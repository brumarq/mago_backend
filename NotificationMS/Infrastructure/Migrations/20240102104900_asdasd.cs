using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class asdasd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Statuses_StatusTypes_StatusTypeId",
                table: "Statuses");

            migrationBuilder.AlterColumn<int>(
                name: "StatusTypeId",
                table: "Statuses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Statuses_StatusTypes_StatusTypeId",
                table: "Statuses",
                column: "StatusTypeId",
                principalTable: "StatusTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Statuses_StatusTypes_StatusTypeId",
                table: "Statuses");

            migrationBuilder.AlterColumn<int>(
                name: "StatusTypeId",
                table: "Statuses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Statuses_StatusTypes_StatusTypeId",
                table: "Statuses",
                column: "StatusTypeId",
                principalTable: "StatusTypes",
                principalColumn: "Id");
        }
    }
}
