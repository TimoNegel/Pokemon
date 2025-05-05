using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pokemon.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePokemonModel4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Hauptfähigkeit", table: "PokemonsCache");

            migrationBuilder.DropColumn(name: "VersteckteFähigkeit", table: "PokemonsCache");

            migrationBuilder.RenameColumn(
                name: "Verteidigung",
                table: "PokemonsCache",
                newName: "Speed"
            );

            migrationBuilder.RenameColumn(name: "KP", table: "PokemonsCache", newName: "Defense");

            migrationBuilder.RenameColumn(
                name: "Angriff",
                table: "PokemonsCache",
                newName: "Attack"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Speed",
                table: "PokemonsCache",
                newName: "Verteidigung"
            );

            migrationBuilder.RenameColumn(name: "Defense", table: "PokemonsCache", newName: "KP");

            migrationBuilder.RenameColumn(
                name: "Attack",
                table: "PokemonsCache",
                newName: "Angriff"
            );

            migrationBuilder.AddColumn<string>(
                name: "Hauptfähigkeit",
                table: "PokemonsCache",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "VersteckteFähigkeit",
                table: "PokemonsCache",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: ""
            );
        }
    }
}
