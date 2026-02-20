using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class OrderSubAndGrandTotal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Orders",
                newName: "SubTotal");

            migrationBuilder.AddColumn<decimal>(
                name: "GrandTotal",
                table: "Orders",
                type: "decimal(28,18)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrandTotal",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "SubTotal",
                table: "Orders",
                newName: "TotalAmount");
        }
    }
}
