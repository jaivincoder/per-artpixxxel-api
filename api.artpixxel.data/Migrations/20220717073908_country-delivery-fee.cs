using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class countrydeliveryfee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "States",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryFee",
                table: "Countries",
                type: "decimal(28,18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "States");

            migrationBuilder.DropColumn(
                name: "DeliveryFee",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Cities");
        }
    }
}
