using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "WarehouseTransferRequestId",
                table: "ExportWarehouseReceipt",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WarehouseTransferRequestId",
                table: "ExportTransactionDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExportWarehouseReceipt_WarehouseTransferRequestId",
                table: "ExportWarehouseReceipt",
                column: "WarehouseTransferRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ExportTransactionDetail_WarehouseTransferRequestId",
                table: "ExportTransactionDetail",
                column: "WarehouseTransferRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportTransactionDetail_WarehouseTransferRequest_WarehouseTransferRequestId",
                table: "ExportTransactionDetail",
                column: "WarehouseTransferRequestId",
                principalTable: "WarehouseTransferRequest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportWarehouseReceipt_WarehouseTransferRequest_WarehouseTransferRequestId",
                table: "ExportWarehouseReceipt",
                column: "WarehouseTransferRequestId",
                principalTable: "WarehouseTransferRequest",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportTransactionDetail_WarehouseTransferRequest_WarehouseTransferRequestId",
                table: "ExportTransactionDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ExportWarehouseReceipt_WarehouseTransferRequest_WarehouseTransferRequestId",
                table: "ExportWarehouseReceipt");

            migrationBuilder.DropIndex(
                name: "IX_ExportWarehouseReceipt_WarehouseTransferRequestId",
                table: "ExportWarehouseReceipt");

            migrationBuilder.DropIndex(
                name: "IX_ExportTransactionDetail_WarehouseTransferRequestId",
                table: "ExportTransactionDetail");

            migrationBuilder.DropColumn(
                name: "WarehouseTransferRequestId",
                table: "ExportWarehouseReceipt");

            migrationBuilder.DropColumn(
                name: "WarehouseTransferRequestId",
                table: "ExportTransactionDetail");
        }
    }
}
