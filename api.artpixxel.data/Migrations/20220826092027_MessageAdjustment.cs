using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class MessageAdjustment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentAbsPath",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentFileType",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentRelPath",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentAbsPath",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "AttachmentFileType",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "AttachmentRelPath",
                table: "Messages");
        }
    }
}
