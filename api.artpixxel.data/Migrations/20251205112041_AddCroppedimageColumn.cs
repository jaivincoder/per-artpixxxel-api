using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.artpixxel.Data.Migrations
{
    public partial class AddCroppedimageColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CroppedImageAbsURL",
                table: "OrderItemImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CroppedImageRelURL",
                table: "OrderItemImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CroppedImageURL",
                table: "OrderItemImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CroppedImageAbsURL",
                table: "KidsGalleryImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CroppedImageRelURL",
                table: "KidsGalleryImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CroppedImageURL",
                table: "KidsGalleryImages",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CroppedImageAbsURL",
                table: "OrderItemImages");

            migrationBuilder.DropColumn(
                name: "CroppedImageRelURL",
                table: "OrderItemImages");

            migrationBuilder.DropColumn(
                name: "CroppedImageURL",
                table: "OrderItemImages");

            migrationBuilder.DropColumn(
                name: "CroppedImageAbsURL",
                table: "KidsGalleryImages");

            migrationBuilder.DropColumn(
                name: "CroppedImageRelURL",
                table: "KidsGalleryImages");

            migrationBuilder.DropColumn(
                name: "CroppedImageURL",
                table: "KidsGalleryImages");
        }
    }
}
