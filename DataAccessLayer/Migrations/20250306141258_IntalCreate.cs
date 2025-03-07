using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class IntalCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "ImportTransactionDetail",
                newName: "TotalPrice");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "ImportTransactionDetail",
                newName: "TotalQuantity");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "WarehouseReceipt",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "WarehouseReceipt");

            migrationBuilder.RenameColumn(
                name: "TotalQuantity",
                table: "ImportTransactionDetail",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "ImportTransactionDetail",
                newName: "TotalAmount");
        }
    }
}
