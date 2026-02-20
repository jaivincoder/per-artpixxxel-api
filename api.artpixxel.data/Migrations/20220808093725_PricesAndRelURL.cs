using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class PricesAndRelURL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageRelURL",
                table: "WallArts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageRelURL",
                table: "WallArtImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageRelURL",
                table: "WallArtCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceImageRelURL",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageRelURL",
                table: "MixnMatches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "MixnMatches",
                type: "decimal(28,18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ImageRelURL",
                table: "Carousels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportRelURL",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    PriceType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(28,18)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropColumn(
                name: "ImageRelURL",
                table: "WallArts");

            migrationBuilder.DropColumn(
                name: "ImageRelURL",
                table: "WallArtImages");

            migrationBuilder.DropColumn(
                name: "ImageRelURL",
                table: "WallArtCategories");

            migrationBuilder.DropColumn(
                name: "SourceImageRelURL",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ImageRelURL",
                table: "MixnMatches");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "MixnMatches");

            migrationBuilder.DropColumn(
                name: "ImageRelURL",
                table: "Carousels");

            migrationBuilder.DropColumn(
                name: "PassportRelURL",
                table: "AspNetUsers");
        }
    }
}
