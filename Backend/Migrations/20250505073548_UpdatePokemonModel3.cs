using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pokemon.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePokemonModel3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Entwicklung",
                table: "PokemonsCache",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]"
            );

            migrationBuilder.CreateTable(
                name: "Moves",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Schaden = table.Column<int>(type: "int", nullable: false),
                    LevelLearnedAt = table.Column<int>(type: "int", nullable: false),
                    Typ = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moves", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "PokemonMove",
                columns: table => new
                {
                    MoveId = table.Column<int>(type: "int", nullable: false),
                    PokemonId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonMove", x => new { x.MoveId, x.PokemonId });
                    table.ForeignKey(
                        name: "FK_PokemonMove_Moves_MoveId",
                        column: x => x.MoveId,
                        principalTable: "Moves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_PokemonMove_PokemonsCache_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "PokemonsCache",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_PokemonMove_PokemonId",
                table: "PokemonMove",
                column: "PokemonId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PokemonMove");

            migrationBuilder.DropTable(name: "Moves");

            migrationBuilder.DropColumn(name: "Entwicklung", table: "PokemonsCache");
        }
    }
}
