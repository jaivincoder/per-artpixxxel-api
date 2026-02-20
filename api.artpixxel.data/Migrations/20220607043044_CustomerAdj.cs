using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class CustomerAdj : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_CustomerCategories_TypeId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "Customers",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_TypeId",
                table: "Customers",
                newName: "IX_Customers_CategoryId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "CustomerCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_CustomerCategories_CategoryId",
                table: "Customers",
                column: "CategoryId",
                principalTable: "CustomerCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_CustomerCategories_CategoryId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "CustomerCategories");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Customers",
                newName: "TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_CategoryId",
                table: "Customers",
                newName: "IX_Customers_TypeId");

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_CustomerCategories_TypeId",
                table: "Customers",
                column: "TypeId",
                principalTable: "CustomerCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
