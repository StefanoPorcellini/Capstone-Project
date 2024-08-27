using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestioneOrdini.Migrations
{
    /// <inheritdoc />
    public partial class editDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dimensions_LaserStandards_LaserStandardId",
                table: "Dimensions");

            migrationBuilder.DropForeignKey(
                name: "FK_Dimensions_PlotterStandards_PlotterStandardId",
                table: "Dimensions");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Dimensions_ItemId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dimensions",
                table: "Dimensions");

            migrationBuilder.RenameTable(
                name: "Dimensions",
                newName: "Item");

            migrationBuilder.RenameIndex(
                name: "IX_Dimensions_PlotterStandardId",
                table: "Item",
                newName: "IX_Item_PlotterStandardId");

            migrationBuilder.RenameIndex(
                name: "IX_Dimensions_LaserStandardId",
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
                name: "FK_Orders_Item_ItemId",
                table: "Orders",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_LaserStandards_LaserStandardId",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_Item_PlotterStandards_PlotterStandardId",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Item_ItemId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Item",
                table: "Item");

            migrationBuilder.RenameTable(
                name: "Item",
                newName: "Dimensions");

            migrationBuilder.RenameIndex(
                name: "IX_Item_PlotterStandardId",
                table: "Dimensions",
                newName: "IX_Dimensions_PlotterStandardId");

            migrationBuilder.RenameIndex(
                name: "IX_Item_LaserStandardId",
                table: "Dimensions",
                newName: "IX_Dimensions_LaserStandardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dimensions",
                table: "Dimensions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dimensions_LaserStandards_LaserStandardId",
                table: "Dimensions",
                column: "LaserStandardId",
                principalTable: "LaserStandards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Dimensions_PlotterStandards_PlotterStandardId",
                table: "Dimensions",
                column: "PlotterStandardId",
                principalTable: "PlotterStandards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Dimensions_ItemId",
                table: "Orders",
                column: "ItemId",
                principalTable: "Dimensions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
