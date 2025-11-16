using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Entities; // Your correct namespace

namespace WorkoutService.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider sp)
        {
            var ctx = sp.GetRequiredService<ApplicationDbContext>();

            await SeedWorkoutPlansAsync(ctx);
            await SeedExercisesAsync(ctx);
            await SeedWorkoutsAsync(ctx);
        }

        private static async Task SeedWorkoutPlansAsync(ApplicationDbContext ctx)
        {
            if (await ctx.WorkoutPlans.AnyAsync()) return;

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
            if (await ctx.Exercises.AnyAsync()) return;

            var exercises = new List<Exercise>
            {
                new() {
                    Name = "Push-up",
                    Description = "A basic calisthenic exercise for upper body strength.",
                    Difficulty = "Beginner",
                    TargetMuscles = new List<string> { "Chest", "Triceps", "Shoulders" },
                    EquipmentNeeded = new List<string> { "Bodyweight" }
                },
                new() {
                    Name = "Bodyweight Squat",
                    Description = "A fundamental lower body exercise.",
                    Difficulty = "Beginner",
                    TargetMuscles = new List<string> { "Quads", "Glutes", "Hamstrings" },
                    EquipmentNeeded = new List<string> { "Bodyweight" }
                },
                new() {
                    Name = "Plank",
                    Description = "An isometric core strength exercise.",
                    Difficulty = "Beginner",
                    TargetMuscles = new List<string> { "Core", "Abs" },
                    EquipmentNeeded = new List<string> { "Bodyweight" }
                },
                new() {
                    Name = "Dumbbell Bench Press",
                    Description = "A chest-building exercise using dumbbells.",
                    Difficulty = "Intermediate",
                    TargetMuscles = new List<string> { "Chest", "Triceps" },
                    EquipmentNeeded = new List<string> { "Dumbbells", "Bench" }
                },
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
            if (await ctx.Workouts.AnyAsync()) return;

            var planBeginner = await ctx.WorkoutPlans.FirstOrDefaultAsync(p => p.ExternalPlanId == "plan_fit_beginner");
            var planNormal = await ctx.WorkoutPlans.FirstOrDefaultAsync(p => p.ExternalPlanId == "plan_lw_normal");

            var pushup = await ctx.Exercises.FirstOrDefaultAsync(e => e.Name == "Push-up");
            var squat = await ctx.Exercises.FirstOrDefaultAsync(e => e.Name == "Bodyweight Squat");
            var plank = await ctx.Exercises.FirstOrDefaultAsync(e => e.Name == "Plank");
            var dbPress = await ctx.Exercises.FirstOrDefaultAsync(e => e.Name == "Dumbbell Bench Press");
            var dbRow = await ctx.Exercises.FirstOrDefaultAsync(e => e.Name == "Dumbbell Row");

            if (planBeginner == null || planNormal == null || pushup == null || squat == null || plank == null || dbPress == null || dbRow == null)
            {
                return; // Can't seed
            }

            var workouts = new List<Workout>
            {
                new() {
                    Name = "Full Body Introduction",
                    Description = "A workout to learn the basic movements.",
                    Category = "full-body",
                    Difficulty = "Beginner",
                    DurationInMinutes = 20,
                    CaloriesBurn = 150,
                    IsPremium = false,
                    Rating = 0,
                    WorkoutPlan = planBeginner, // --- FIX: Use navigation property
                    WorkoutExercises = new List<WorkoutExercise>
                    {
                        new() { Exercise = squat, Order = 1, Sets = 3, Reps = "8-12", RestTimeInSeconds = 60 },
                        new() { Exercise = pushup, Order = 2, Sets = 3, Reps = "5-10 (Knees OK)", RestTimeInSeconds = 60 },
                        new() { Exercise = plank, Order = 3, Sets = 3, Reps = "30s", RestTimeInSeconds = 45 }
                    }
                },
                new() {
                    Name = "Strength & Cardio Mix",
                    Description = "A routine to burn calories and build muscle.",
                    Category = "full-body",
                    Difficulty = "Intermediate",
                    DurationInMinutes = 45,
                    CaloriesBurn = 350,
                    IsPremium = false,
                    Rating = 0,
                    WorkoutPlan = planNormal, // --- FIX: Use navigation property
                    WorkoutExercises = new List<WorkoutExercise>
                    {
                        new() { Exercise = squat, Order = 1, Sets = 3, Reps = "12-15", RestTimeInSeconds = 60 },
                        new() { Exercise = dbPress, Order = 2, Sets = 3, Reps = "10-12", RestTimeInSeconds = 60 },
                        new() { Exercise = dbRow, Order = 3, Sets = 3, Reps = "10-12 (each side)", RestTimeInSeconds = 60 },
                        new() { Exercise = plank, Order = 4, Sets = 3, Reps = "60s", RestTimeInSeconds = 45 }
                    }
                }
            };

            await ctx.Workouts.AddRangeAsync(workouts);
            await ctx.SaveChangesAsync();
        }
    }
}