using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.artpixxel.Data.Migrations
{
    public partial class FestiveDesignsAddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FestiveDesigns",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageAbsURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageRelURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_FestiveDesigns", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FestiveDesigns");
        }
    }
}
