using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class orderItemTotalAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "OrderItems",
                type: "decimal(28,18)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "OrderItems");
        }
    }
}
