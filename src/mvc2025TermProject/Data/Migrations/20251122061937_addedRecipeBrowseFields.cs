using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvc2025TermProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedRecipeBrowseFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NutritionInfo",
                table: "Recipes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecialEquipment",
                table: "Recipes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YouTubeVideoLink",
                table: "Recipes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "RecipeIngredients",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NutritionInfo",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "SpecialEquipment",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "YouTubeVideoLink",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "RecipeIngredients");
        }
    }
}
