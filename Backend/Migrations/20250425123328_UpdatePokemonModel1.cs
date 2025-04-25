using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pokemon.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePokemonModel1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Art",
                table: "PokemonsCache",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "Beschreibung",
                table: "PokemonsCache",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "Generation",
                table: "PokemonsCache",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<int>(
                name: "Geschlecht",
                table: "PokemonsCache",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "Gewicht",
                table: "PokemonsCache",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "Groesse",
                table: "PokemonsCache",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<string>(
                name: "ResistenzGegen",
                table: "PokemonsCache",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]"
            );

            migrationBuilder.AddColumn<string>(
                name: "SchwächeGegen",
                table: "PokemonsCache",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Art", table: "PokemonsCache");

            migrationBuilder.DropColumn(name: "Beschreibung", table: "PokemonsCache");

            migrationBuilder.DropColumn(name: "Generation", table: "PokemonsCache");

            migrationBuilder.DropColumn(name: "Geschlecht", table: "PokemonsCache");

            migrationBuilder.DropColumn(name: "Gewicht", table: "PokemonsCache");

            migrationBuilder.DropColumn(name: "Groesse", table: "PokemonsCache");

            migrationBuilder.DropColumn(name: "ResistenzGegen", table: "PokemonsCache");

            migrationBuilder.DropColumn(name: "SchwächeGegen", table: "PokemonsCache");
        }
    }
}
