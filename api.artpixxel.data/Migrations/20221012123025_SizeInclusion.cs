using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class SizeInclusion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WallArts_WallArtSizes_SizeId",
                table: "WallArts");

            migrationBuilder.RenameColumn(
                name: "SizeId",
                table: "WallArts",
                newName: "WallArtSizeId");

            migrationBuilder.RenameIndex(
                name: "IX_WallArts_SizeId",
                table: "WallArts",
                newName: "IX_WallArts_WallArtSizeId");

            migrationBuilder.AddColumn<string>(
                name: "SizeName",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WallArts_WallArtSizes_WallArtSizeId",
                table: "WallArts",
                column: "WallArtSizeId",
                principalTable: "WallArtSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WallArts_WallArtSizes_WallArtSizeId",
                table: "WallArts");

            migrationBuilder.DropColumn(
                name: "SizeName",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "WallArtSizeId",
                table: "WallArts",
                newName: "SizeId");

            migrationBuilder.RenameIndex(
                name: "IX_WallArts_WallArtSizeId",
                table: "WallArts",
                newName: "IX_WallArts_SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_WallArts_WallArtSizes_SizeId",
                table: "WallArts",
                column: "SizeId",
                principalTable: "WallArtSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
