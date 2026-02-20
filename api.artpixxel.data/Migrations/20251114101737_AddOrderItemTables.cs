using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.artpixxel.Data.Migrations
{
    public partial class AddOrderItemTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderCartItems",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TotalAmountPerCategory = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FrameId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateConfigId = table.Column<int>(type: "int", nullable: true),
                    FrameClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LineColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviewImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UniqueItemId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    FrameSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCartItems", x => x.id);
                    table.ForeignKey(
                        name: "FK_OrderCartItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderCartItemImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderCartItemId = table.Column<int>(type: "int", nullable: false),
                    CroppedItemImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalItemImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCartItemImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderCartItemImages_OrderCartItems_OrderCartItemId",
                        column: x => x.OrderCartItemId,
                        principalTable: "OrderCartItems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderCartItemImages_OrderCartItemId",
                table: "OrderCartItemImages",
                column: "OrderCartItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCartItems_OrderId",
                table: "OrderCartItems",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderCartItemImages");

            migrationBuilder.DropTable(
                name: "OrderCartItems");
        }
    }
}
