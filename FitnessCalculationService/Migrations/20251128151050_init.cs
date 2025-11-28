using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessCalculationService.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeightGoalActivity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    Height = table.Column<double>(type: "float", nullable: false),
                    ActivityLevel = table.Column<int>(type: "int", nullable: false),
                    Goal = table.Column<int>(type: "int", nullable: false),
                    InsertDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeightGoalActivity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutPlandb",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurationWeeks = table.Column<int>(type: "int", nullable: false),
                    Difficulty = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutPlandb", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserFitnessStat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Bmr = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tdee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CalorieTarget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InsertDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WeightGoalActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFitnessStat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFitnessStat_WeightGoalActivity_WeightGoalActivityId",
                        column: x => x.WeightGoalActivityId,
                        principalTable: "WeightGoalActivity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FitnessPlanConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Goal = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MinCalorie = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MaxCalorie = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    WorkoutPlanId = table.Column<int>(type: "int", nullable: false),
                    PlanType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    WorkoutPlanId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FitnessPlanConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FitnessPlanConfig_WorkoutPlandb_WorkoutPlanId1",
                        column: x => x.WorkoutPlanId1,
                        principalTable: "WorkoutPlandb",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FitnessPlanConfig_WorkoutPlanId1",
                table: "FitnessPlanConfig",
                column: "WorkoutPlanId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserFitnessStat_WeightGoalActivityId",
                table: "UserFitnessStat",
                column: "WeightGoalActivityId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FitnessPlanConfig");

            migrationBuilder.DropTable(
                name: "UserFitnessStat");

            migrationBuilder.DropTable(
                name: "WorkoutPlandb");

            migrationBuilder.DropTable(
                name: "WeightGoalActivity");
        }
    }
}
