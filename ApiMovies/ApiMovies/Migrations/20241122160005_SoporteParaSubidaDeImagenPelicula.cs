using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiMovies.Migrations
{
    /// <inheritdoc />
    public partial class SoporteParaSubidaDeImagenPelicula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocalImageRoute",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalImageRoute",
                table: "Movies");
        }
    }
}
