using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pokemon.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PokemonsCache",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BildUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Primärtyp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sekundärtyp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Habitat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hauptfähigkeit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VersteckteFähigkeit = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false
                    ),
                    KP = table.Column<int>(type: "int", nullable: false),
                    HP = table.Column<int>(type: "int", nullable: false),
                    Angriff = table.Column<int>(type: "int", nullable: false),
                    Verteidigung = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonsCache", x => x.Id);
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PokemonsCache");
        }
    }
}
