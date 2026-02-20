using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class OrderItemSizes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SizeId",
                table: "OrderItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_SizeId",
                table: "OrderItems",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Sizes_SizeId",
                table: "OrderItems",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Sizes_SizeId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_SizeId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "OrderItems");
        }
    }
}
