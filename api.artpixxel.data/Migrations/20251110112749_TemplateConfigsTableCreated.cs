using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.artpixxel.Data.Migrations
{
    public partial class TemplateConfigsTableCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Frames",
                type: "nvarchar(450)",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWID()");

            migrationBuilder.CreateTable(
                name: "TemplateConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    PreviewImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_TemplateConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateConfigs_FrameCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "FrameCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemplateConfigs_CategoryId",
                table: "TemplateConfigs",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateConfigs");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Frames",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldDefaultValueSql: "NEWID()");
        }
    }
}
