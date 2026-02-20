using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.artpixxel.Data.Migrations
{
    public partial class IsActiveAddColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TemplateFramesMapping",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TemplateConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "line_colors_Master",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "GalleryImages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Frames",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FrameCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TemplateFramesMapping");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TemplateConfigs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "line_colors_Master");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "GalleryImages");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Frames");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FrameCategories");
        }
    }
}
