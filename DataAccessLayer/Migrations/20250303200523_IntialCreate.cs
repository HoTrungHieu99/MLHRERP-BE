using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class IntialCreate : Migration
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
