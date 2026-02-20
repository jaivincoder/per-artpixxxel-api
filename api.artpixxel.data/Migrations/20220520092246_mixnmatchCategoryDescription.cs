using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class mixnmatchCategoryDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MixnMatchCategories",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "MixnMatchCategories");
        }
    }
}
