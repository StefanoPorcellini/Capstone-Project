using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestioneOrdini.Migrations
{
    /// <inheritdoc />
    public partial class UpdeteDbPlotterList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCustom",
                table: "Item",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PlotterItem_IsCustom",
                table: "Item",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlotterItem_Quantity",
                table: "Item",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LaserPriceLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MinQuantity = table.Column<int>(type: "int", nullable: false),
                    MaxQuantity = table.Column<int>(type: "int", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LaserItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaserPriceLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaserPriceLists_Item_LaserItemId",
                        column: x => x.LaserItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaserPriceLists_LaserItemId",
                table: "LaserPriceLists",
                column: "LaserItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LaserPriceLists");

            migrationBuilder.DropColumn(
                name: "IsCustom",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "PlotterItem_IsCustom",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "PlotterItem_Quantity",
                table: "Item");
        }
    }
}
