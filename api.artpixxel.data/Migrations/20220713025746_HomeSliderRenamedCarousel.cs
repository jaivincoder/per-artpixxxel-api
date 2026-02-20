using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class HomeSliderRenamedCarousel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeSliders");

            migrationBuilder.CreateTable(
                name: "Carousels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Heading = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkLabel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Carousels", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Carousels");

            migrationBuilder.CreateTable(
                name: "HomeSliders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    BodyText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Heading = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkLabel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeSliders", x => x.Id);
                });
        }
    }
}
