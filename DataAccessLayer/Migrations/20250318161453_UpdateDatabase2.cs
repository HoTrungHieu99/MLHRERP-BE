using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Order_OrderId1",
                table: "OrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetail_OrderId1",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                table: "OrderDetail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId1",
                table: "OrderDetail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderId1",
                table: "OrderDetail",
                column: "OrderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Order_OrderId1",
                table: "OrderDetail",
                column: "OrderId1",
                principalTable: "Order",
                principalColumn: "OrderId");
        }
    }
}
