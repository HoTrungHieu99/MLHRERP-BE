using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderCode",
                table: "WarehouseTransferRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestExportId",
                table: "WarehouseTransferRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransferRequest_RequestExportId",
                table: "WarehouseTransferRequest",
                column: "RequestExportId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseTransferRequest_RequestExport_RequestExportId",
                table: "WarehouseTransferRequest",
                column: "RequestExportId",
                principalTable: "RequestExport",
                principalColumn: "RequestExportId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseTransferRequest_RequestExport_RequestExportId",
                table: "WarehouseTransferRequest");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseTransferRequest_RequestExportId",
                table: "WarehouseTransferRequest");

            migrationBuilder.DropColumn(
                name: "OrderCode",
                table: "WarehouseTransferRequest");

            migrationBuilder.DropColumn(
                name: "RequestExportId",
                table: "WarehouseTransferRequest");
        }
    }
}
