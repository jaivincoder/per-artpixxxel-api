using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace api.artpixxel.Data.Migrations
{
    public partial class OrderInvoiceNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderStatusItems_StatusId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "OrderStatusItems");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "OrderStatuses");

            migrationBuilder.DropColumn(
                name: "FixedSize",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Images",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Orders",
                newName: "OrderState");

            migrationBuilder.RenameColumn(
                name: "SourceImageURL",
                table: "OrderItems",
                newName: "ImageURL");

            migrationBuilder.RenameColumn(
                name: "SourceImageRelURL",
                table: "OrderItems",
                newName: "ImageRelURL");

            migrationBuilder.RenameColumn(
                name: "SourceImageAbsURL",
                table: "OrderItems",
                newName: "ImageAbsURL");

            migrationBuilder.RenameColumn(
                name: "SourceImage",
                table: "OrderItems",
                newName: "Image");

            migrationBuilder.AddColumn<string>(
                name: "ColorCode",
                table: "OrderStatuses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "OrderStatuses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Label",
                table: "OrderStatuses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StateId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MixnMatchId",
                table: "OrderItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WallArtId",
                table: "OrderItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderHistories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StatusId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_OrderHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderHistories_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderHistories_OrderStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "OrderStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemImages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageAbsURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageRelURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderItemId = table.Column<string>(type: "nvarchar(450)", nullable: true),
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
                    table.PrimaryKey("PK_OrderItemImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemImages_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CityId",
                table: "Orders",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CountryId",
                table: "Orders",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_StateId",
                table: "Orders",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_MixnMatchId",
                table: "OrderItems",
                column: "MixnMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_WallArtId",
                table: "OrderItems",
                column: "WallArtId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHistories_OrderId",
                table: "OrderHistories",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHistories_StatusId",
                table: "OrderHistories",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemImages_OrderItemId",
                table: "OrderItemImages",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_MixnMatches_MixnMatchId",
                table: "OrderItems",
                column: "MixnMatchId",
                principalTable: "MixnMatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_WallArts_WallArtId",
                table: "OrderItems",
                column: "WallArtId",
                principalTable: "WallArts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Cities_CityId",
                table: "Orders",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Countries_CountryId",
                table: "Orders",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderStatuses_StatusId",
                table: "Orders",
                column: "StatusId",
                principalTable: "OrderStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_States_StateId",
                table: "Orders",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_MixnMatches_MixnMatchId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_WallArts_WallArtId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Cities_CityId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Countries_CountryId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderStatuses_StatusId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_States_StateId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "OrderHistories");

            migrationBuilder.DropTable(
                name: "OrderItemImages");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CityId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CountryId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_StateId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_MixnMatchId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_WallArtId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ColorCode",
                table: "OrderStatuses");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "OrderStatuses");

            migrationBuilder.DropColumn(
                name: "Label",
                table: "OrderStatuses");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MixnMatchId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "WallArtId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "OrderState",
                table: "Orders",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "ImageURL",
                table: "OrderItems",
                newName: "SourceImageURL");

            migrationBuilder.RenameColumn(
                name: "ImageRelURL",
                table: "OrderItems",
                newName: "SourceImageRelURL");

            migrationBuilder.RenameColumn(
                name: "ImageAbsURL",
                table: "OrderItems",
                newName: "SourceImageAbsURL");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "OrderItems",
                newName: "SourceImage");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "OrderStatuses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "FixedSize",
                table: "OrderItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Images",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderStatusItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    ColorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Forward = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrderStatusId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatusItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderStatusItems_OrderStatuses_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalTable: "OrderStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusItems_OrderStatusId",
                table: "OrderStatusItems",
                column: "OrderStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderStatusItems_StatusId",
                table: "Orders",
                column: "StatusId",
                principalTable: "OrderStatusItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
