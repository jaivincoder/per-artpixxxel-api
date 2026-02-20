using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.artpixxel.Data.Migrations
{
    public partial class TitleTOGalleryTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "FrameCategories");

            migrationBuilder.AddColumn<string>(
                name: "GallaryTitle",
                table: "FrameCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GallaryTitle",
                table: "FrameCategories");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "FrameCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
