using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.artpixxel.Data.Migrations
{
    public partial class FramCategoriesUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_line_colors_Master_FrameCategories_CategoryId",
                table: "line_colors_Master");

            migrationBuilder.AddForeignKey(
                name: "FK_line_colors_Master_FrameCategories_CategoryId",
                table: "line_colors_Master",
                column: "CategoryId",
                principalTable: "FrameCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_line_colors_Master_FrameCategories_CategoryId",
                table: "line_colors_Master");

            migrationBuilder.AddForeignKey(
                name: "FK_line_colors_Master_FrameCategories_CategoryId",
                table: "line_colors_Master",
                column: "CategoryId",
                principalTable: "FrameCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
