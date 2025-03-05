using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_Employee_SalesAgentId",
                table: "Request");

            migrationBuilder.RenameColumn(
                name: "SalesAgentId",
                table: "Request",
                newName: "AgencyId");

            migrationBuilder.RenameIndex(
                name: "IX_Request_SalesAgentId",
                table: "Request",
                newName: "IX_Request_AgencyId");

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_Image_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Image_ProductId",
                table: "Image",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_AgencyAccount_AgencyId",
                table: "Request",
                column: "AgencyId",
                principalTable: "AgencyAccount",
                principalColumn: "AgencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_AgencyAccount_AgencyId",
                table: "Request");

            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.RenameColumn(
                name: "AgencyId",
                table: "Request",
                newName: "SalesAgentId");

            migrationBuilder.RenameIndex(
                name: "IX_Request_AgencyId",
                table: "Request",
                newName: "IX_Request_SalesAgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Employee_SalesAgentId",
                table: "Request",
                column: "SalesAgentId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");
        }
    }
}
