using System;
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
            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedBy",
                table: "WarehouseRequestExport",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "SellingPrice",
                table: "Batch",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<decimal>(
                name: "ProfitMarginPercent",
                table: "Batch",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseRequestExport_ApprovedBy",
                table: "WarehouseRequestExport",
                column: "ApprovedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseRequestExport_User_ApprovedBy",
                table: "WarehouseRequestExport",
                column: "ApprovedBy",
                principalTable: "User",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseRequestExport_User_ApprovedBy",
                table: "WarehouseRequestExport");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseRequestExport_ApprovedBy",
                table: "WarehouseRequestExport");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "WarehouseRequestExport");

            migrationBuilder.DropColumn(
                name: "ProfitMarginPercent",
                table: "Batch");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellingPrice",
                table: "Batch",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);
        }
    }
}
