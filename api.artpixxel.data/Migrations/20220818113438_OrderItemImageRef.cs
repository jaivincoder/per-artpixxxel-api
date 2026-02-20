using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class OrderItemImageRef : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WallArtImageId",
                table: "OrderItemImages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemImages_WallArtImageId",
                table: "OrderItemImages",
                column: "WallArtImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemImages_WallArtImages_WallArtImageId",
                table: "OrderItemImages",
                column: "WallArtImageId",
                principalTable: "WallArtImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemImages_WallArtImages_WallArtImageId",
                table: "OrderItemImages");

            migrationBuilder.DropIndex(
                name: "IX_OrderItemImages_WallArtImageId",
                table: "OrderItemImages");

            migrationBuilder.DropColumn(
                name: "WallArtImageId",
                table: "OrderItemImages");
        }
    }
}
