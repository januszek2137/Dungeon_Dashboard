using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dungeon_Dashboard.Migrations
{
    /// <inheritdoc />
    public partial class characterModelRace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Race",
                table: "CharacterModel",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Race",
                table: "CharacterModel");
        }
    }
}
