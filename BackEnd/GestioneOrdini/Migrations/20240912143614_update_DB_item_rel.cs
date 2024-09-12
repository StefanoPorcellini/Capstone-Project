using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestioneOrdini.Migrations
{
    /// <inheritdoc />
    public partial class update_DB_item_rel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_WorkTypes_WorkTypeId",
                table: "Items");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_WorkTypes_WorkTypeId",
                table: "Items",
                column: "WorkTypeId",
                principalTable: "WorkTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_WorkTypes_WorkTypeId",
                table: "Items");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_WorkTypes_WorkTypeId",
                table: "Items",
                column: "WorkTypeId",
                principalTable: "WorkTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
