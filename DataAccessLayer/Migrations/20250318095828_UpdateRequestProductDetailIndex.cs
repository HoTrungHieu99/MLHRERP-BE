using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequestProductDetailIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RequestProductDetail_RequestProductId",
                table: "RequestProductDetail");

            migrationBuilder.CreateIndex(
                name: "IX_RequestProductDetail_RequestProductId_ProductId",
                table: "RequestProductDetail",
                columns: new[] { "RequestProductId", "ProductId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RequestProductDetail_RequestProductId_ProductId",
                table: "RequestProductDetail");

            migrationBuilder.CreateIndex(
                name: "IX_RequestProductDetail_RequestProductId",
                table: "RequestProductDetail",
                column: "RequestProductId");
        }
    }
}
