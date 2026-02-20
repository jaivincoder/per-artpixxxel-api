using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class NotificationSubjectId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "Notifications",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "SubjectId",
                table: "Notifications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SubjectId",
                table: "Notifications",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_SubjectId",
                table: "Notifications",
                column: "SubjectId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_SubjectId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_SubjectId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Notifications",
                newName: "Subject");
        }
    }
}
