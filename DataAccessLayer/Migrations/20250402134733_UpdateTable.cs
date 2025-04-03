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
            migrationBuilder.DropPrimaryKey(
                name: "PK_OTPEmail",
                table: "OTPEmail");

            migrationBuilder.RenameTable(
                name: "OTPEmail",
                newName: "OTPEmails");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OTPEmails",
                table: "OTPEmails",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "WarehouseTransferRequest",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceWarehouseId = table.Column<long>(type: "bigint", nullable: true),
                    DestinationWarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlannedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseTransferRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseTransferRequest_User_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "User",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_WarehouseTransferRequest_User_PlannedBy",
                        column: x => x.PlannedBy,
                        principalTable: "User",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_WarehouseTransferRequest_User_RequestedBy",
                        column: x => x.RequestedBy,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WarehouseTransferRequest_Warehouse_DestinationWarehouseId",
                        column: x => x.DestinationWarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "WarehouseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarehouseTransferRequest_Warehouse_SourceWarehouseId",
                        column: x => x.SourceWarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "WarehouseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseTransferProduct",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseTransferRequestId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseTransferProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseTransferProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WarehouseTransferProduct_WarehouseTransferRequest_WarehouseTransferRequestId",
                        column: x => x.WarehouseTransferRequestId,
                        principalTable: "WarehouseTransferRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransferProduct_ProductId",
                table: "WarehouseTransferProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransferProduct_WarehouseTransferRequestId",
                table: "WarehouseTransferProduct",
                column: "WarehouseTransferRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransferRequest_ApprovedBy",
                table: "WarehouseTransferRequest",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransferRequest_DestinationWarehouseId",
                table: "WarehouseTransferRequest",
                column: "DestinationWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransferRequest_PlannedBy",
                table: "WarehouseTransferRequest",
                column: "PlannedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransferRequest_RequestedBy",
                table: "WarehouseTransferRequest",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransferRequest_SourceWarehouseId",
                table: "WarehouseTransferRequest",
                column: "SourceWarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarehouseTransferProduct");

            migrationBuilder.DropTable(
                name: "WarehouseTransferRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OTPEmails",
                table: "OTPEmails");

            migrationBuilder.RenameTable(
                name: "OTPEmails",
                newName: "OTPEmail");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OTPEmail",
                table: "OTPEmail",
                column: "Id");
        }
    }
}
