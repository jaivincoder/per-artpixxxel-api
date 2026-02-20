using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class OrderHistory_Adjust : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColorCode",
                table: "OrderHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "OrderHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Label",
                table: "OrderHistories",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorCode",
                table: "OrderHistories");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "OrderHistories");

            migrationBuilder.DropColumn(
                name: "Label",
                table: "OrderHistories");
        }
    }
}
