using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestioneOrdini.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbTableOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_LaserStandards_LaserStandardId",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_Item_PlotterStandards_PlotterStandardId",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_LaserPriceLists_Item_LaserItemId",
                table: "LaserPriceLists");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Item_ItemId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Item",
                table: "Item");

            migrationBuilder.RenameTable(
                name: "Item",
                newName: "Items");

            migrationBuilder.RenameIndex(
                name: "IX_Item_PlotterStandardId",
                table: "Items",
                newName: "IX_Items_PlotterStandardId");

            migrationBuilder.RenameIndex(
                name: "IX_Item_LaserStandardId",
                table: "Items",
                newName: "IX_Items_LaserStandardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_LaserStandards_LaserStandardId",
                table: "Items",
                column: "LaserStandardId",
                principalTable: "LaserStandards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_PlotterStandards_PlotterStandardId",
                table: "Items",
                column: "PlotterStandardId",
                principalTable: "PlotterStandards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaserPriceLists_Items_LaserItemId",
                table: "LaserPriceLists",
                column: "LaserItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Items_ItemId",
                table: "Orders",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_LaserStandards_LaserStandardId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_PlotterStandards_PlotterStandardId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_LaserPriceLists_Items_LaserItemId",
                table: "LaserPriceLists");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Items_ItemId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.RenameTable(
                name: "Items",
                newName: "Item");

            migrationBuilder.RenameIndex(
                name: "IX_Items_PlotterStandardId",
                table: "Item",
                newName: "IX_Item_PlotterStandardId");

            migrationBuilder.RenameIndex(
                name: "IX_Items_LaserStandardId",
                table: "Item",
                newName: "IX_Item_LaserStandardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Item",
                table: "Item",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_LaserStandards_LaserStandardId",
                table: "Item",
                column: "LaserStandardId",
                principalTable: "LaserStandards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Item_PlotterStandards_PlotterStandardId",
                table: "Item",
                column: "PlotterStandardId",
                principalTable: "PlotterStandards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaserPriceLists_Item_LaserItemId",
                table: "LaserPriceLists",
                column: "LaserItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Item_ItemId",
                table: "Orders",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
