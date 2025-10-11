using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFirstAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDKategori",
                table: "Produk");

            migrationBuilder.RenameColumn(
                name: "harga",
                table: "Produk",
                newName: "Harga");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Harga",
                table: "Produk",
                newName: "harga");

            migrationBuilder.AddColumn<int>(
                name: "IDKategori",
                table: "Produk",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
