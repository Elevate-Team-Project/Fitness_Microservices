using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Entities;
using WorkoutService.Infrastructure.Data;

namespace WorkoutService.Infrastructure.Data
{
    /// <summary>
    /// Handles seeding the database with initial, essential data.
    /// This is modeled after your TechZone project's seeder.
    /// </summary>
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider sp)
        {
            // Get the DbContext from the service provider
            var ctx = sp.GetRequiredService<ApplicationDbContext>();

            // --- Note ---
            // We are not seeding Roles or Users here, as this service
            // does not manage Identity.

            // 1. Seed Workout Plans
            await SeedWorkoutPlansAsync(ctx);

            // 2. Seed Exercises
            await SeedExercisesAsync(ctx);

            // 3. Seed Workouts and link them to exercises
            await SeedWorkoutsAsync(ctx);
        }

        private static async Task SeedWorkoutPlansAsync(ApplicationDbContext ctx)
        {
            if (await ctx.WorkoutPlans.AnyAsync()) return; // Do not re-seed

            var plans = new List<WorkoutPlan>
            {
                new() {
                    ExternalPlanId = "plan_lw_normal",
                    Name = "Weight Loss - Normal Intensity",
                    Description = "A balanced plan for steady weight loss.",
                    Goal = "Lose Weight",
                    Status = "Normal",
                    Difficulty = "Intermediate"
                },
                new() {
                    ExternalPlanId = "plan_gw_hard",
                    Name = "Gain Weight - Hard Intensity",
                    Description = "A high-volume plan for building mass.",
                    Goal = "Gain Weight",
                    Status = "Hard",
                    Difficulty = "Advanced"
                },
                new() {
                    ExternalPlanId = "plan_fit_beginner",
                    Name = "Get Fitter - Beginner",
                    Description = "An introduction to fitness fundamentals.",
                    Goal = "Get Fitter",
                    Status = "Normal",
                    Difficulty = "Beginner"
                }
            };

            await ctx.WorkoutPlans.AddRangeAsync(plans);
            await ctx.SaveChangesAsync();
        }

        private static async Task SeedExercisesAsync(ApplicationDbContext ctx)
        {
            if (await ctx.Exercises.AnyAsync()) return; // Do not re-seed

            var exercises = new List<Exercise>
            {
                // 1
                new() {
                    Name = "Push-up",
                    Description = "A basic calisthenic exercise for upper body strength.",
                    Difficulty = "Beginner",
                    TargetMuscles = new List<string> { "Chest", "Triceps", "Shoulders" },
                    EquipmentNeeded = new List<string> { "Bodyweight" }
                },
                // 2
                new() {
                    Name = "Bodyweight Squat",
                    Description = "A fundamental lower body exercise.",
                    Difficulty = "Beginner",
                    TargetMuscles = new List<string> { "Quads", "Glutes", "Hamstrings" },
                    EquipmentNeeded = new List<string> { "Bodyweight" }
                },
                // 3
                new() {
                    Name = "Plank",
                    Description = "An isometric core strength exercise.",
                    Difficulty = "Beginner",
                    TargetMuscles = new List<string> { "Core", "Abs" },
                    EquipmentNeeded = new List<string> { "Bodyweight" }
                },
                // 4
                new() {
                    Name = "Dumbbell Bench Press",
                    Description = "A chest-building exercise using dumbbells.",
                    Difficulty = "Intermediate",
                    TargetMuscles = new List<string> { "Chest", "Triceps" },
                    EquipmentNeeded = new List<string> { "Dumbbells", "Bench" }
                },
                // 5
                new() {
                    Name = "Dumbbell Row",
                    Description = "A back-building exercise using a single dumbbell.",
                    Difficulty = "Intermediate",
                    TargetMuscles = new List<string> { "Back", "Lats", "Biceps" },
                    EquipmentNeeded = new List<string> { "Dumbbells", "Bench" }
                }
            };

            await ctx.Exercises.AddRangeAsync(exercises);
            await ctx.SaveChangesAsync();
        }

        private static async Task SeedWorkoutsAsync(ApplicationDbContext ctx)
        {
            if (await ctx.Workouts.AnyAsync()) return; // Do not re-seed

            // We need to fetch the plans and exercises we just created
            // to link them properly.
            var planBeginner = await ctx.WorkoutPlans.FirstOrDefaultAsync(p => p.ExternalPlanId == "plan_fit_beginner");
            var planNormal = await ctx.WorkoutPlans.FirstOrDefaultAsync(p => p.ExternalPlanId == "plan_lw_normal");

            var pushup = await ctx.Exercises.FirstOrDefaultAsync(e => e.Name == "Push-up");
            var squat = await ctx.Exercises.FirstOrDefaultAsync(e => e.Name == "Bodyweight Squat");
            var plank = await ctx.Exercises.FirstOrDefaultAsync(e => e.Name == "Plank");
            var dbPress = await ctx.Exercises.FirstOrDefaultAsync(e => e.Name == "Dumbbell Bench Press");
            var dbRow = await ctx.Exercises.FirstOrDefaultAsync(e => e.Name == "Dumbbell Row");

            // Ensure our seed data exists before proceeding
            if (planBeginner == null || planNormal == null || pushup == null || squat == null || plank == null || dbPress == null || dbRow == null)
            {
                // Log an error or throw an exception if seed data is missing
                return;
            }

            var workouts = new List<Workout>
            {
                // Workout 1: Full Body Beginner
                new() {
                    Name = "Full Body Introduction",
                    Description = "A workout to learn the basic movements.",
                    Category = "full-body",
                    Difficulty = "Beginner",
                    DurationInMinutes = 20,
                    CaloriesBurn = 150,
                    IsPremium = false,
                    Rating = 0,
                    WorkoutPlanId = planBeginner.Id,
                    WorkoutExercises = new List<WorkoutExercise>
                    {
                        new() { ExerciseId = squat.Id, Order = 1, Sets = 3, Reps = "8-12", RestTimeInSeconds = 60 },
                        new() { ExerciseId = pushup.Id, Order = 2, Sets = 3, Reps = "5-10 (Knees OK)", RestTimeInSeconds = 60 },
                        new() { ExerciseId = plank.Id, Order = 3, Sets = 3, Reps = "30s", RestTimeInSeconds = 45 }
                    }
                },
                // Workout 2: Weight Loss Normal
                new() {
                    Name = "Strength & Cardio Mix",
                    Description = "A routine to burn calories and build muscle.",
                    Category = "full-body",
                    Difficulty = "Intermediate",
                    DurationInMinutes = 45,
                    CaloriesBurn = 350,
                    IsPremium = false,
                    Rating = 0,
                    WorkoutPlanId = planNormal.Id,
                    WorkoutExercises = new List<WorkoutExercise>
                    {
                        new() { ExerciseId = squat.Id, Order = 1, Sets = 3, Reps = "12-15", RestTimeInSeconds = 60 },
                        new() { ExerciseId = dbPress.Id, Order = 2, Sets = 3, Reps = "10-12", RestTimeInSeconds = 60 },
                        new() { ExerciseId = dbRow.Id, Order = 3, Sets = 3, Reps = "10-12 (each side)", RestTimeInSeconds = 60 },
                        new() { ExerciseId = plank.Id, Order = 4, Sets = 3, Reps = "60s", RestTimeInSeconds = 45 }
                    }
                }
            };

            await ctx.Workouts.AddRangeAsync(workouts);
            await ctx.SaveChangesAsync();
        }
    }
}