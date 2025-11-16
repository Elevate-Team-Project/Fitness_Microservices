using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WorkoutService.Domain.Entities; // Your correct namespace

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
        public DbSet<WorkoutSession> WorkoutSessions { get; set; }
        public DbSet<WorkoutSessionExercise> WorkoutSessionExercises { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- THIS IS THE FIX ---
            // We tell EF that Id (from BaseEntity) is the Primary Key by *not* calling HasKey.
            // Instead, we create a UNIQUE INDEX on these columns to prevent duplicates.
            modelBuilder.Entity<WorkoutExercise>()
                .HasIndex(we => new { we.WorkoutId, we.ExerciseId, we.Order })
                .IsUnique();
            // --- END OF FIX ---

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

            // Add Global Query Filter for Soft Delete
            modelBuilder.Entity<Workout>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Exercise>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<WorkoutPlan>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<WorkoutExercise>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<WorkoutSession>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<WorkoutSessionExercise>().HasQueryFilter(e => !e.IsDeleted);
        }

        // Override SaveChanges to handle soft delete and timestamps
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                // This will now work for ALL entities, including WorkoutExercise
                if (entry.Entity is BaseEntity baseEntity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            baseEntity.CreatedAt = DateTime.UtcNow;
                            baseEntity.IsDeleted = false;
                            break;
                        case EntityState.Modified:
                            baseEntity.UpdatedAt = DateTime.UtcNow;
                            break;
                        case EntityState.Deleted:
                            entry.State = EntityState.Modified; // Don't actually delete
                            baseEntity.IsDeleted = true;
                            baseEntity.DeletedAt = DateTime.UtcNow;
                            break;
                    }
                }
            }
        }
    }
}