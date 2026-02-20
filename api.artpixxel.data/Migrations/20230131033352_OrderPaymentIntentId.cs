using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.artpixxel.Data.Migrations
{
    public partial class OrderPaymentIntentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChargeId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "TokenId",
                table: "Orders",
                newName: "PaymentIntentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentIntentId",
                table: "Orders",
                newName: "TokenId");

            migrationBuilder.AddColumn<string>(
                name: "ChargeId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
