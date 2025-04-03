﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTable5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportTransactionDetail_WarehouseTransferRequest_WarehouseTransferRequestId",
                table: "ExportTransactionDetail");

            migrationBuilder.DropIndex(
                name: "IX_ExportTransactionDetail_WarehouseTransferRequestId",
                table: "ExportTransactionDetail");

            migrationBuilder.DropColumn(
                name: "WarehouseTransferRequestId",
                table: "ExportTransactionDetail");

            migrationBuilder.AddColumn<long>(
                name: "WarehouseTransferRequestId",
                table: "ExportTransaction",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExportTransaction_WarehouseTransferRequestId",
                table: "ExportTransaction",
                column: "WarehouseTransferRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportTransaction_WarehouseTransferRequest_WarehouseTransferRequestId",
                table: "ExportTransaction",
                column: "WarehouseTransferRequestId",
                principalTable: "WarehouseTransferRequest",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportTransaction_WarehouseTransferRequest_WarehouseTransferRequestId",
                table: "ExportTransaction");

            migrationBuilder.DropIndex(
                name: "IX_ExportTransaction_WarehouseTransferRequestId",
                table: "ExportTransaction");

            migrationBuilder.DropColumn(
                name: "WarehouseTransferRequestId",
                table: "ExportTransaction");

            migrationBuilder.AddColumn<long>(
                name: "WarehouseTransferRequestId",
                table: "ExportTransactionDetail",
                type: "bigint",
                nullable: true);

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
        }
    }
}
