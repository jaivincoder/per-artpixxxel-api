using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class Order_Adj : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "VAT",
                table: "Orders",
                type: "decimal(28,18)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryFee",
                table: "Orders",
                type: "decimal(28,18)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryFee",
                table: "Orders");

            migrationBuilder.AlterColumn<double>(
                name: "VAT",
                table: "Orders",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,18)");
        }
    }
}
