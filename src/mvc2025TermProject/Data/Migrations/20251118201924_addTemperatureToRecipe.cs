using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvc2025TermProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class addTemperatureToRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TemperatureFahrenheit",
                table: "Recipes",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemperatureFahrenheit",
                table: "Recipes");
        }
    }
}
