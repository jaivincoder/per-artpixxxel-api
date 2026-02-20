using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.artpixxel.Data.Migrations
{
    public partial class OrderItemPrevImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LineColor",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LineStyle",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PreviewImage",
                table: "OrderItems",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviewImageAbsURL",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviewImageURL",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LineColor",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "LineStyle",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "PreviewImage",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "PreviewImageAbsURL",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "PreviewImageURL",
                table: "OrderItems");
        }
    }
}
