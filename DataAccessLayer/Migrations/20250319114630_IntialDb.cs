using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class IntialDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Warehouse_User_UserId",
                table: "Warehouse");

            migrationBuilder.DropIndex(
                name: "IX_Warehouse_UserId",
                table: "Warehouse");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Warehouse");

            migrationBuilder.CreateTable(
                name: "WarehouseManager",
                columns: table => new
                {
                    WarehouseManagerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseManager", x => x.WarehouseManagerId);
                    table.ForeignKey(
                        name: "FK_WarehouseManager_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WarehouseManager_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "WarehouseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseManager_UserId",
                table: "WarehouseManager",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseManager_WarehouseId",
                table: "WarehouseManager",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarehouseManager");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Warehouse",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_UserId",
                table: "Warehouse",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouse_User_UserId",
                table: "Warehouse",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
