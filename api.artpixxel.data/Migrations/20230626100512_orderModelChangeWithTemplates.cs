using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.artpixxel.Data.Migrations
{
    public partial class orderModelChangeWithTemplates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KidsTemplateSizeId",
                table: "OrderItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MixedTemplateSizeId",
                table: "OrderItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TempId",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_KidsTemplateSizeId",
                table: "OrderItems",
                column: "KidsTemplateSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_MixedTemplateSizeId",
                table: "OrderItems",
                column: "MixedTemplateSizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_KidsTemplateSizes_KidsTemplateSizeId",
                table: "OrderItems",
                column: "KidsTemplateSizeId",
                principalTable: "KidsTemplateSizes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_MixedTemplateSizes_MixedTemplateSizeId",
                table: "OrderItems",
                column: "MixedTemplateSizeId",
                principalTable: "MixedTemplateSizes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_KidsTemplateSizes_KidsTemplateSizeId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_MixedTemplateSizes_MixedTemplateSizeId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_KidsTemplateSizeId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_MixedTemplateSizeId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "KidsTemplateSizeId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "MixedTemplateSizeId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "TempId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "OrderItems");
        }
    }
}
