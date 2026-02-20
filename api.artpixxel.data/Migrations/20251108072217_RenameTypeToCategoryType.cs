using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.artpixxel.Data.Migrations
{
    public partial class RenameTypeToCategoryType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "FrameCategories",
                newName: "CategoryType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CategoryType",
                table: "FrameCategories",
                newName: "Type");
        }
    }
}
