using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.artpixxel.Data.Migrations
{
    public partial class CustomerAddressBookChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CityName",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityName",
                table: "AddressBooks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryId",
                table: "AddressBooks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StateId",
                table: "AddressBooks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AddressBooks_CountryId",
                table: "AddressBooks",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressBooks_StateId",
                table: "AddressBooks",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_AddressBooks_Countries_CountryId",
                table: "AddressBooks",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AddressBooks_States_StateId",
                table: "AddressBooks",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddressBooks_Countries_CountryId",
                table: "AddressBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_AddressBooks_States_StateId",
                table: "AddressBooks");

            migrationBuilder.DropIndex(
                name: "IX_AddressBooks_CountryId",
                table: "AddressBooks");

            migrationBuilder.DropIndex(
                name: "IX_AddressBooks_StateId",
                table: "AddressBooks");

            migrationBuilder.DropColumn(
                name: "CityName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CityName",
                table: "AddressBooks");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "AddressBooks");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "AddressBooks");
        }
    }
}
