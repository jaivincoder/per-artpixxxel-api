using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.artpixxel.Data.Migrations
{
    public partial class OrderItemAdj : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChristmasTemplateSizeId",
                table: "OrderItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegularTemplateSizeId",
                table: "OrderItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ChristmasTemplateSizeId",
                table: "OrderItems",
                column: "ChristmasTemplateSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_RegularTemplateSizeId",
                table: "OrderItems",
                column: "RegularTemplateSizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ChristmasTemplateSizes_ChristmasTemplateSizeId",
                table: "OrderItems",
                column: "ChristmasTemplateSizeId",
                principalTable: "ChristmasTemplateSizes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_RegularTemplateSizes_RegularTemplateSizeId",
                table: "OrderItems",
                column: "RegularTemplateSizeId",
                principalTable: "RegularTemplateSizes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ChristmasTemplateSizes_ChristmasTemplateSizeId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_RegularTemplateSizes_RegularTemplateSizeId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ChristmasTemplateSizeId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_RegularTemplateSizeId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ChristmasTemplateSizeId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "RegularTemplateSizeId",
                table: "OrderItems");
        }
    }
}
