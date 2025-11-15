using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Text.Json;
using WorkoutService.Domain.Entities;

namespace WorkoutService.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Workout> Workouts { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure WorkoutExercise composite primary key
            modelBuilder.Entity<WorkoutExercise>()
                .HasKey(we => new { we.WorkoutId, we.ExerciseId, we.Order }); // Added Order to key to ensure uniqueness per step

            // Configure many-to-many relationship between Workout and Exercise
            modelBuilder.Entity<WorkoutExercise>()
                .HasOne(we => we.Workout)
                .WithMany(w => w.WorkoutExercises)
                .HasForeignKey(we => we.WorkoutId);

            modelBuilder.Entity<WorkoutExercise>()
                .HasOne(we => we.Exercise)
                .WithMany(e => e.WorkoutExercises)
                .HasForeignKey(we => we.ExerciseId);

            // Configure one-to-many relationship between WorkoutPlan and Workout
            modelBuilder.Entity<Workout>()
                .HasOne(w => w.WorkoutPlan)
                .WithMany(wp => wp.Workouts)
                .HasForeignKey(w => w.WorkoutPlanId);

            // Configure List<string> to JSON string conversion for Exercise entity
            var jsonSerializerOptions = new JsonSerializerOptions();

            modelBuilder.Entity<Exercise>()
                .Property(e => e.TargetMuscles)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonSerializerOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, jsonSerializerOptions) ?? new List<string>()
                );

            modelBuilder.Entity<Exercise>()
                .Property(e => e.EquipmentNeeded)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonSerializerOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, jsonSerializerOptions) ?? new List<string>()
                );
        }
    }
}