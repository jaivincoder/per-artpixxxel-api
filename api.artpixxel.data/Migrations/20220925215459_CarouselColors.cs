using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class CarouselColors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackgroundColour",
                table: "Carousels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyTextColour",
                table: "Carousels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeadingColour",
                table: "Carousels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkLabelColour",
                table: "Carousels",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundColour",
                table: "Carousels");

            migrationBuilder.DropColumn(
                name: "BodyTextColour",
                table: "Carousels");

            migrationBuilder.DropColumn(
                name: "HeadingColour",
                table: "Carousels");

            migrationBuilder.DropColumn(
                name: "LinkLabelColour",
                table: "Carousels");
        }
    }
}
