using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class Sizes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_WallArtSizes_SizeId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "SizeId",
                table: "OrderItems",
                newName: "WallArtSizeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_SizeId",
                table: "OrderItems",
                newName: "IX_OrderItems_WallArtSizeId");

            migrationBuilder.CreateTable(
                name: "Sizes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Width = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(28,18)", nullable: false),
                    Default = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Sizes", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_WallArtSizes_WallArtSizeId",
                table: "OrderItems",
                column: "WallArtSizeId",
                principalTable: "WallArtSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_WallArtSizes_WallArtSizeId",
                table: "OrderItems");

            migrationBuilder.DropTable(
                name: "Sizes");

            migrationBuilder.RenameColumn(
                name: "WallArtSizeId",
                table: "OrderItems",
                newName: "SizeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_WallArtSizeId",
                table: "OrderItems",
                newName: "IX_OrderItems_SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_WallArtSizes_SizeId",
                table: "OrderItems",
                column: "SizeId",
                principalTable: "WallArtSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
