using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class ImageAbsoluteURL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageAbsURL",
                table: "WallArts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageAbsURL",
                table: "WallArtImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageAbsURL",
                table: "WallArtCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CroppedImageAbsURL",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CroppedImageURL",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceImageAbsURL",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageAbsURL",
                table: "MixnMatches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageAbsURL",
                table: "Carousels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportAbsURL",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageAbsURL",
                table: "WallArts");

            migrationBuilder.DropColumn(
                name: "ImageAbsURL",
                table: "WallArtImages");

            migrationBuilder.DropColumn(
                name: "ImageAbsURL",
                table: "WallArtCategories");

            migrationBuilder.DropColumn(
                name: "CroppedImageAbsURL",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CroppedImageURL",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "SourceImageAbsURL",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ImageAbsURL",
                table: "MixnMatches");

            migrationBuilder.DropColumn(
                name: "ImageAbsURL",
                table: "Carousels");

            migrationBuilder.DropColumn(
                name: "PassportAbsURL",
                table: "AspNetUsers");
        }
    }
}
