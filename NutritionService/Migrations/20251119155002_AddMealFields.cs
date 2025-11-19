using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutritionService.Migrations
{
    /// <inheritdoc />
    public partial class AddMealFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Meals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Meals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPremium",
                table: "Meals",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Meals");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Meals");

            migrationBuilder.DropColumn(
                name: "IsPremium",
                table: "Meals");
        }
    }
}
