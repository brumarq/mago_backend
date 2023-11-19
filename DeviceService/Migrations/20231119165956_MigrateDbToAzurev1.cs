using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceService.Migrations
{
    /// <inheritdoc />
    public partial class MigrateDbToAzurev1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceLocations_Devices_DeviceId",
                table: "DeviceLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_DeviceLocations_Locations_LocationId",
                table: "DeviceLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceTypes_DeviceTypeId",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Quantities_Units_BaseUnitId",
                table: "Quantities");

            migrationBuilder.DropForeignKey(
                name: "FK_Settings_DeviceTypes_DeviceTypeId",
                table: "Settings");

            migrationBuilder.DropForeignKey(
                name: "FK_Settings_Units_UnitId",
                table: "Settings");

            migrationBuilder.DropForeignKey(
                name: "FK_SettingValues_Devices_DeviceId",
                table: "SettingValues");

            migrationBuilder.DropForeignKey(
                name: "FK_SettingValues_Settings_SettingId",
                table: "SettingValues");

            migrationBuilder.DropForeignKey(
                name: "FK_Units_Quantities_QuantityId",
                table: "Units");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersOnDevices_Devices_DeviceId",
                table: "UsersOnDevices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Units",
                table: "Units");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SettingValues",
                table: "SettingValues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Settings",
                table: "Settings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quantities",
                table: "Quantities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Locations",
                table: "Locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeviceTypes",
                table: "DeviceTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Devices",
                table: "Devices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeviceLocations",
                table: "DeviceLocations");

            migrationBuilder.RenameTable(
                name: "Units",
                newName: "Unit");

            migrationBuilder.RenameTable(
                name: "SettingValues",
                newName: "SettingValue");

            migrationBuilder.RenameTable(
                name: "Settings",
                newName: "Setting");

            migrationBuilder.RenameTable(
                name: "Quantities",
                newName: "Quantity");

            migrationBuilder.RenameTable(
                name: "Locations",
                newName: "Location");

            migrationBuilder.RenameTable(
                name: "DeviceTypes",
                newName: "DeviceType");

            migrationBuilder.RenameTable(
                name: "Devices",
                newName: "Device");

            migrationBuilder.RenameTable(
                name: "DeviceLocations",
                newName: "DeviceLocation");

            migrationBuilder.RenameIndex(
                name: "IX_Units_QuantityId",
                table: "Unit",
                newName: "IX_Unit_QuantityId");

            migrationBuilder.RenameIndex(
                name: "IX_SettingValues_SettingId",
                table: "SettingValue",
                newName: "IX_SettingValue_SettingId");

            migrationBuilder.RenameIndex(
                name: "IX_SettingValues_DeviceId",
                table: "SettingValue",
                newName: "IX_SettingValue_DeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_Settings_UnitId",
                table: "Setting",
                newName: "IX_Setting_UnitId");

            migrationBuilder.RenameIndex(
                name: "IX_Settings_DeviceTypeId",
                table: "Setting",
                newName: "IX_Setting_DeviceTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Quantities_BaseUnitId",
                table: "Quantity",
                newName: "IX_Quantity_BaseUnitId");

            migrationBuilder.RenameIndex(
                name: "IX_Devices_DeviceTypeId",
                table: "Device",
                newName: "IX_Device_DeviceTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_DeviceLocations_LocationId",
                table: "DeviceLocation",
                newName: "IX_DeviceLocation_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_DeviceLocations_DeviceId",
                table: "DeviceLocation",
                newName: "IX_DeviceLocation_DeviceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Unit",
                table: "Unit",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SettingValue",
                table: "SettingValue",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Setting",
                table: "Setting",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quantity",
                table: "Quantity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Location",
                table: "Location",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeviceType",
                table: "DeviceType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Device",
                table: "Device",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeviceLocation",
                table: "DeviceLocation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_DeviceType_DeviceTypeId",
                table: "Device",
                column: "DeviceTypeId",
                principalTable: "DeviceType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceLocation_Device_DeviceId",
                table: "DeviceLocation",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceLocation_Location_LocationId",
                table: "DeviceLocation",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quantity_Unit_BaseUnitId",
                table: "Quantity",
                column: "BaseUnitId",
                principalTable: "Unit",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Setting_DeviceType_DeviceTypeId",
                table: "Setting",
                column: "DeviceTypeId",
                principalTable: "DeviceType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Setting_Unit_UnitId",
                table: "Setting",
                column: "UnitId",
                principalTable: "Unit",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SettingValue_Device_DeviceId",
                table: "SettingValue",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SettingValue_Setting_SettingId",
                table: "SettingValue",
                column: "SettingId",
                principalTable: "Setting",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Unit_Quantity_QuantityId",
                table: "Unit",
                column: "QuantityId",
                principalTable: "Quantity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersOnDevices_Device_DeviceId",
                table: "UsersOnDevices",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Device_DeviceType_DeviceTypeId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_DeviceLocation_Device_DeviceId",
                table: "DeviceLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_DeviceLocation_Location_LocationId",
                table: "DeviceLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_Quantity_Unit_BaseUnitId",
                table: "Quantity");

            migrationBuilder.DropForeignKey(
                name: "FK_Setting_DeviceType_DeviceTypeId",
                table: "Setting");

            migrationBuilder.DropForeignKey(
                name: "FK_Setting_Unit_UnitId",
                table: "Setting");

            migrationBuilder.DropForeignKey(
                name: "FK_SettingValue_Device_DeviceId",
                table: "SettingValue");

            migrationBuilder.DropForeignKey(
                name: "FK_SettingValue_Setting_SettingId",
                table: "SettingValue");

            migrationBuilder.DropForeignKey(
                name: "FK_Unit_Quantity_QuantityId",
                table: "Unit");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersOnDevices_Device_DeviceId",
                table: "UsersOnDevices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Unit",
                table: "Unit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SettingValue",
                table: "SettingValue");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Setting",
                table: "Setting");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quantity",
                table: "Quantity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Location",
                table: "Location");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeviceType",
                table: "DeviceType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeviceLocation",
                table: "DeviceLocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Device",
                table: "Device");

            migrationBuilder.RenameTable(
                name: "Unit",
                newName: "Units");

            migrationBuilder.RenameTable(
                name: "SettingValue",
                newName: "SettingValues");

            migrationBuilder.RenameTable(
                name: "Setting",
                newName: "Settings");

            migrationBuilder.RenameTable(
                name: "Quantity",
                newName: "Quantities");

            migrationBuilder.RenameTable(
                name: "Location",
                newName: "Locations");

            migrationBuilder.RenameTable(
                name: "DeviceType",
                newName: "DeviceTypes");

            migrationBuilder.RenameTable(
                name: "DeviceLocation",
                newName: "DeviceLocations");

            migrationBuilder.RenameTable(
                name: "Device",
                newName: "Devices");

            migrationBuilder.RenameIndex(
                name: "IX_Unit_QuantityId",
                table: "Units",
                newName: "IX_Units_QuantityId");

            migrationBuilder.RenameIndex(
                name: "IX_SettingValue_SettingId",
                table: "SettingValues",
                newName: "IX_SettingValues_SettingId");

            migrationBuilder.RenameIndex(
                name: "IX_SettingValue_DeviceId",
                table: "SettingValues",
                newName: "IX_SettingValues_DeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_Setting_UnitId",
                table: "Settings",
                newName: "IX_Settings_UnitId");

            migrationBuilder.RenameIndex(
                name: "IX_Setting_DeviceTypeId",
                table: "Settings",
                newName: "IX_Settings_DeviceTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Quantity_BaseUnitId",
                table: "Quantities",
                newName: "IX_Quantities_BaseUnitId");

            migrationBuilder.RenameIndex(
                name: "IX_DeviceLocation_LocationId",
                table: "DeviceLocations",
                newName: "IX_DeviceLocations_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_DeviceLocation_DeviceId",
                table: "DeviceLocations",
                newName: "IX_DeviceLocations_DeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_Device_DeviceTypeId",
                table: "Devices",
                newName: "IX_Devices_DeviceTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Units",
                table: "Units",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SettingValues",
                table: "SettingValues",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Settings",
                table: "Settings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quantities",
                table: "Quantities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Locations",
                table: "Locations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeviceTypes",
                table: "DeviceTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeviceLocations",
                table: "DeviceLocations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Devices",
                table: "Devices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceLocations_Devices_DeviceId",
                table: "DeviceLocations",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceLocations_Locations_LocationId",
                table: "DeviceLocations",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_DeviceTypes_DeviceTypeId",
                table: "Devices",
                column: "DeviceTypeId",
                principalTable: "DeviceTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quantities_Units_BaseUnitId",
                table: "Quantities",
                column: "BaseUnitId",
                principalTable: "Units",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Settings_DeviceTypes_DeviceTypeId",
                table: "Settings",
                column: "DeviceTypeId",
                principalTable: "DeviceTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Settings_Units_UnitId",
                table: "Settings",
                column: "UnitId",
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
                name: "FK_Units_Quantities_QuantityId",
                table: "Units",
                column: "QuantityId",
                principalTable: "Quantities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersOnDevices_Devices_DeviceId",
                table: "UsersOnDevices",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");
        }
    }
}
