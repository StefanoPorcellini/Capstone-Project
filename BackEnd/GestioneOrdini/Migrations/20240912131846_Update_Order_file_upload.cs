using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestioneOrdini.Migrations
{
    /// <inheritdoc />
    public partial class Update_Order_file_upload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Items",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Items",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Items");
        }
    }
}
