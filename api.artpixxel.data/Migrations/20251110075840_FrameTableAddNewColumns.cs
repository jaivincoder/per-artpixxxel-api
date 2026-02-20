using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.artpixxel.Data.Migrations
{
    public partial class FrameTableAddNewColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Frames",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Frames",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FrameId",
                table: "Frames",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Frames_CategoryId",
                table: "Frames",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Frames_FrameCategories_CategoryId",
                table: "Frames",
                column: "CategoryId",
                principalTable: "FrameCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Frames_FrameCategories_CategoryId",
                table: "Frames");

            migrationBuilder.DropIndex(
                name: "IX_Frames_CategoryId",
                table: "Frames");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Frames");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Frames");

            migrationBuilder.DropColumn(
                name: "FrameId",
                table: "Frames");
        }
    }
}
