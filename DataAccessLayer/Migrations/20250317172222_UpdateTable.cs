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
                name: "FK_RequestProduct_Product_ProductId",
                table: "RequestProduct");

            migrationBuilder.DropIndex(
                name: "IX_RequestProduct_ProductId",
                table: "RequestProduct");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "RequestProduct");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "RequestProduct",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_RequestProduct_ProductId",
                table: "RequestProduct",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestProduct_Product_ProductId",
                table: "RequestProduct",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
