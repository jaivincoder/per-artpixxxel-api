using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class OrderCustomerInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalPhoneNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobilePhoneNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalPhoneNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MobilePhoneNumber",
                table: "Orders");
        }
    }
}
