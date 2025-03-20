using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportTransactionDetail_Product_ProductId",
                table: "ExportTransactionDetail");

            migrationBuilder.AddColumn<string>(
                name: "AgencyName",
                table: "ExportWarehouseReceipt",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RequestExportId",
                table: "ExportWarehouseReceipt",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "ProductId",
                table: "ExportTransactionDetail",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgencyName",
                table: "ExportTransaction",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RequestExportId",
                table: "ExportTransaction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ExportWarehouseReceipt_RequestExportId",
                table: "ExportWarehouseReceipt",
                column: "RequestExportId");

            migrationBuilder.CreateIndex(
                name: "IX_ExportTransaction_RequestExportId",
                table: "ExportTransaction",
                column: "RequestExportId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportTransaction_RequestExport_RequestExportId",
                table: "ExportTransaction",
                column: "RequestExportId",
                principalTable: "RequestExport",
                principalColumn: "RequestExportId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExportTransactionDetail_Product_ProductId",
                table: "ExportTransactionDetail",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExportWarehouseReceipt_RequestExport_RequestExportId",
                table: "ExportWarehouseReceipt",
                column: "RequestExportId",
                principalTable: "RequestExport",
                principalColumn: "RequestExportId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportTransaction_RequestExport_RequestExportId",
                table: "ExportTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_ExportTransactionDetail_Product_ProductId",
                table: "ExportTransactionDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ExportWarehouseReceipt_RequestExport_RequestExportId",
                table: "ExportWarehouseReceipt");

            migrationBuilder.DropIndex(
                name: "IX_ExportWarehouseReceipt_RequestExportId",
                table: "ExportWarehouseReceipt");

            migrationBuilder.DropIndex(
                name: "IX_ExportTransaction_RequestExportId",
                table: "ExportTransaction");

            migrationBuilder.DropColumn(
                name: "AgencyName",
                table: "ExportWarehouseReceipt");

            migrationBuilder.DropColumn(
                name: "RequestExportId",
                table: "ExportWarehouseReceipt");

            migrationBuilder.DropColumn(
                name: "AgencyName",
                table: "ExportTransaction");

            migrationBuilder.DropColumn(
                name: "RequestExportId",
                table: "ExportTransaction");

            migrationBuilder.AlterColumn<long>(
                name: "ProductId",
                table: "ExportTransactionDetail",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportTransactionDetail_Product_ProductId",
                table: "ExportTransactionDetail",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId");
        }
    }
}
