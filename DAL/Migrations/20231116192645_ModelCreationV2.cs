using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class ModelCreationV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceLocations_Devices_DeviceId",
                table: "DeviceLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_FileSends_Devices_DeviceId",
                table: "FileSends");

            migrationBuilder.DropForeignKey(
                name: "FK_FileSends_Users_UserId",
                table: "FileSends");

            migrationBuilder.DropForeignKey(
                name: "FK_LogCollections_Devices_DeviceId",
                table: "LogCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_LogCollections_LogCollectionTypes_LogCollectionTypeId1",
                table: "LogCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_LogValues_Fields_FieldId",
                table: "LogValues");

            migrationBuilder.DropForeignKey(
                name: "FK_LogValues_LogCollections_LogCollectionId",
                table: "LogValues");

            migrationBuilder.DropForeignKey(
                name: "FK_Quantities_Units_UnitId",
                table: "Quantities");

            migrationBuilder.DropForeignKey(
                name: "FK_SettingValues_Devices_DeviceId",
                table: "SettingValues");

            migrationBuilder.DropForeignKey(
                name: "FK_SettingValues_Settings_SettingId",
                table: "SettingValues");

            migrationBuilder.DropForeignKey(
                name: "FK_SettingValues_Users_UserId",
                table: "SettingValues");

            migrationBuilder.DropForeignKey(
                name: "FK_Statusses_Devices_DeviceId",
                table: "Statusses");

            migrationBuilder.DropForeignKey(
                name: "FK_Statusses_StatusTypes_StatusTypeId1",
                table: "Statusses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserOnStatusTypes_Devices_DeviceId",
                table: "UserOnStatusTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserOnStatusTypes_StatusTypes_StatusTypeId1",
                table: "UserOnStatusTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserOnStatusTypes_Users_UserId",
                table: "UserOnStatusTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersOnDevices_Devices_DeviceId",
                table: "UsersOnDevices");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersOnDevices_Users_UserId",
                table: "UsersOnDevices");

            migrationBuilder.DropIndex(
                name: "IX_UserOnStatusTypes_StatusTypeId1",
                table: "UserOnStatusTypes");

            migrationBuilder.DropIndex(
                name: "IX_Statusses_StatusTypeId1",
                table: "Statusses");

            migrationBuilder.DropIndex(
                name: "IX_LogValues_LogCollectionId",
                table: "LogValues");

            migrationBuilder.DropIndex(
                name: "IX_LogCollections_LogCollectionTypeId1",
                table: "LogCollections");

            migrationBuilder.DropColumn(
                name: "StatusTypeId1",
                table: "UserOnStatusTypes");

            migrationBuilder.DropColumn(
                name: "StatusTypeId1",
                table: "Statusses");

            migrationBuilder.DropColumn(
                name: "LogCollectionId",
                table: "LogValues");

            migrationBuilder.DropColumn(
                name: "LogCollectionTypeId1",
                table: "LogCollections");

            migrationBuilder.RenameColumn(
                name: "UnitId",
                table: "Quantities",
                newName: "BaseUnitId");

            migrationBuilder.RenameIndex(
                name: "IX_Quantities_UnitId",
                table: "Quantities",
                newName: "IX_Quantities_BaseUnitId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UsersOnDevices",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "UsersOnDevices",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserOnStatusTypes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "StatusTypeId",
                table: "UserOnStatusTypes",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "UserOnStatusTypes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "StatusTypeId",
                table: "Statusses",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "Statusses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "SettingValues",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SettingId",
                table: "SettingValues",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "SettingValues",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FieldId",
                table: "LogValues",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CollectionId",
                table: "LogValues",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LogCollectionTypeId",
                table: "LogCollections",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "LogCollections",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "FileSends",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "FileSends",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "DeviceLocations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_UserOnStatusTypes_StatusTypeId",
                table: "UserOnStatusTypes",
                column: "StatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Statusses_StatusTypeId",
                table: "Statusses",
                column: "StatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LogValues_CollectionId",
                table: "LogValues",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_LogCollections_LogCollectionTypeId",
                table: "LogCollections",
                column: "LogCollectionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceLocations_Devices_DeviceId",
                table: "DeviceLocations",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileSends_Devices_DeviceId",
                table: "FileSends",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileSends_Users_UserId",
                table: "FileSends",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LogCollections_Devices_DeviceId",
                table: "LogCollections",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LogCollections_LogCollectionTypes_LogCollectionTypeId",
                table: "LogCollections",
                column: "LogCollectionTypeId",
                principalTable: "LogCollectionTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LogValues_Fields_FieldId",
                table: "LogValues",
                column: "FieldId",
                principalTable: "Fields",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LogValues_LogCollections_CollectionId",
                table: "LogValues",
                column: "CollectionId",
                principalTable: "LogCollections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quantities_Units_BaseUnitId",
                table: "Quantities",
                column: "BaseUnitId",
                principalTable: "Units",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SettingValues_Devices_DeviceId",
                table: "SettingValues",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SettingValues_Settings_SettingId",
                table: "SettingValues",
                column: "SettingId",
                principalTable: "Settings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SettingValues_Users_UserId",
                table: "SettingValues",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Statusses_Devices_DeviceId",
                table: "Statusses",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Statusses_StatusTypes_StatusTypeId",
                table: "Statusses",
                column: "StatusTypeId",
                principalTable: "StatusTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOnStatusTypes_Devices_DeviceId",
                table: "UserOnStatusTypes",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOnStatusTypes_StatusTypes_StatusTypeId",
                table: "UserOnStatusTypes",
                column: "StatusTypeId",
                principalTable: "StatusTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOnStatusTypes_Users_UserId",
                table: "UserOnStatusTypes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersOnDevices_Devices_DeviceId",
                table: "UsersOnDevices",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersOnDevices_Users_UserId",
                table: "UsersOnDevices",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceLocations_Devices_DeviceId",
                table: "DeviceLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_FileSends_Devices_DeviceId",
                table: "FileSends");

            migrationBuilder.DropForeignKey(
                name: "FK_FileSends_Users_UserId",
                table: "FileSends");

            migrationBuilder.DropForeignKey(
                name: "FK_LogCollections_Devices_DeviceId",
                table: "LogCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_LogCollections_LogCollectionTypes_LogCollectionTypeId",
                table: "LogCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_LogValues_Fields_FieldId",
                table: "LogValues");

            migrationBuilder.DropForeignKey(
                name: "FK_LogValues_LogCollections_CollectionId",
                table: "LogValues");

            migrationBuilder.DropForeignKey(
                name: "FK_Quantities_Units_BaseUnitId",
                table: "Quantities");

            migrationBuilder.DropForeignKey(
                name: "FK_SettingValues_Devices_DeviceId",
                table: "SettingValues");

            migrationBuilder.DropForeignKey(
                name: "FK_SettingValues_Settings_SettingId",
                table: "SettingValues");

            migrationBuilder.DropForeignKey(
                name: "FK_SettingValues_Users_UserId",
                table: "SettingValues");

            migrationBuilder.DropForeignKey(
                name: "FK_Statusses_Devices_DeviceId",
                table: "Statusses");

            migrationBuilder.DropForeignKey(
                name: "FK_Statusses_StatusTypes_StatusTypeId",
                table: "Statusses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserOnStatusTypes_Devices_DeviceId",
                table: "UserOnStatusTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserOnStatusTypes_StatusTypes_StatusTypeId",
                table: "UserOnStatusTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserOnStatusTypes_Users_UserId",
                table: "UserOnStatusTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersOnDevices_Devices_DeviceId",
                table: "UsersOnDevices");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersOnDevices_Users_UserId",
                table: "UsersOnDevices");

            migrationBuilder.DropIndex(
                name: "IX_UserOnStatusTypes_StatusTypeId",
                table: "UserOnStatusTypes");

            migrationBuilder.DropIndex(
                name: "IX_Statusses_StatusTypeId",
                table: "Statusses");

            migrationBuilder.DropIndex(
                name: "IX_LogValues_CollectionId",
                table: "LogValues");

            migrationBuilder.DropIndex(
                name: "IX_LogCollections_LogCollectionTypeId",
                table: "LogCollections");

            migrationBuilder.DropColumn(
                name: "CollectionId",
                table: "LogValues");

            migrationBuilder.RenameColumn(
                name: "BaseUnitId",
                table: "Quantities",
                newName: "UnitId");

            migrationBuilder.RenameIndex(
                name: "IX_Quantities_BaseUnitId",
                table: "Quantities",
                newName: "IX_Quantities_UnitId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UsersOnDevices",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "UsersOnDevices",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserOnStatusTypes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StatusTypeId",
                table: "UserOnStatusTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "UserOnStatusTypes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusTypeId1",
                table: "UserOnStatusTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StatusTypeId",
                table: "Statusses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "Statusses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusTypeId1",
                table: "Statusses",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "SettingValues",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SettingId",
                table: "SettingValues",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "SettingValues",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FieldId",
                table: "LogValues",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LogCollectionId",
                table: "LogValues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "LogCollectionTypeId",
                table: "LogCollections",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "LogCollections",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LogCollectionTypeId1",
                table: "LogCollections",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "FileSends",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "FileSends",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "DeviceLocations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserOnStatusTypes_StatusTypeId1",
                table: "UserOnStatusTypes",
                column: "StatusTypeId1");

            migrationBuilder.CreateIndex(
                name: "IX_Statusses_StatusTypeId1",
                table: "Statusses",
                column: "StatusTypeId1");

            migrationBuilder.CreateIndex(
                name: "IX_LogValues_LogCollectionId",
                table: "LogValues",
                column: "LogCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_LogCollections_LogCollectionTypeId1",
                table: "LogCollections",
                column: "LogCollectionTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceLocations_Devices_DeviceId",
                table: "DeviceLocations",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileSends_Devices_DeviceId",
                table: "FileSends",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileSends_Users_UserId",
                table: "FileSends",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LogCollections_Devices_DeviceId",
                table: "LogCollections",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LogCollections_LogCollectionTypes_LogCollectionTypeId1",
                table: "LogCollections",
                column: "LogCollectionTypeId1",
                principalTable: "LogCollectionTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LogValues_Fields_FieldId",
                table: "LogValues",
                column: "FieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LogValues_LogCollections_LogCollectionId",
                table: "LogValues",
                column: "LogCollectionId",
                principalTable: "LogCollections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quantities_Units_UnitId",
                table: "Quantities",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SettingValues_Devices_DeviceId",
                table: "SettingValues",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SettingValues_Settings_SettingId",
                table: "SettingValues",
                column: "SettingId",
                principalTable: "Settings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SettingValues_Users_UserId",
                table: "SettingValues",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Statusses_Devices_DeviceId",
                table: "Statusses",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Statusses_StatusTypes_StatusTypeId1",
                table: "Statusses",
                column: "StatusTypeId1",
                principalTable: "StatusTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOnStatusTypes_Devices_DeviceId",
                table: "UserOnStatusTypes",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserOnStatusTypes_StatusTypes_StatusTypeId1",
                table: "UserOnStatusTypes",
                column: "StatusTypeId1",
                principalTable: "StatusTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOnStatusTypes_Users_UserId",
                table: "UserOnStatusTypes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersOnDevices_Devices_DeviceId",
                table: "UsersOnDevices",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersOnDevices_Users_UserId",
                table: "UsersOnDevices",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
