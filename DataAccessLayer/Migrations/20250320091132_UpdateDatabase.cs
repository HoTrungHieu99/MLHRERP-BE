using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "PaymentHistory",
                newName: "TotalAmountPayment");

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentAmount",
                table: "PaymentHistory",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentAmount",
                table: "PaymentHistory");

            migrationBuilder.RenameColumn(
                name: "TotalAmountPayment",
                table: "PaymentHistory",
                newName: "Amount");
        }
    }
}
