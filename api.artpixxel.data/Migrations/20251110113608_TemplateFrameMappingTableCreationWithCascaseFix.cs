using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.artpixxel.Data.Migrations
{
    public partial class TemplateFrameMappingTableCreationWithCascaseFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TemplateFramesMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateConfigId = table.Column<int>(type: "int", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    FrameId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_TemplateFramesMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateFramesMapping_Frames_FrameId",
                        column: x => x.FrameId,
                        principalTable: "Frames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateFramesMapping_TemplateConfigs_TemplateConfigId",
                        column: x => x.TemplateConfigId,
                        principalTable: "TemplateConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemplateFramesMapping_FrameId",
                table: "TemplateFramesMapping",
                column: "FrameId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateFramesMapping_TemplateConfigId",
                table: "TemplateFramesMapping",
                column: "TemplateConfigId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateFramesMapping");
        }
    }
}
