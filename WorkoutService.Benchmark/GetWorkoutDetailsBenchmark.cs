using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Entities;
using WorkoutService.Features.Workouts.GetWorkoutDetails;
using WorkoutService.Infrastructure;
using WorkoutService.Infrastructure.Data;


namespace WorkoutService.Benchmark
{
    [MemoryDiagnoser]
    public class GetWorkoutDetailsBenchmark
    {
        private ApplicationDbContext _dbContext;
        private GetWorkoutDetailsHandler _handler;
        private GetWorkoutDetailsQuery _query;

        [GlobalSetup]
        public void Setup()
        {
            // 1. Configure EF Core to use InMemory Database
            // We use a unique GUID for the database name to ensure isolation between runs if needed.
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);

            // 2. Seed Data
            // We populate the InMemory database with the necessary entities.
            var workoutPlan = new WorkoutPlan
            {
                Id = 1,
                Name = "Test Plan",
                Description = "Test Description",
                Goal = "Strength",
                Difficulty = "Intermediate",
                Status = "Active",
                ExternalPlanId = "plan_123"
            };
            _dbContext.WorkoutPlans.Add(workoutPlan);

            var exercise = new Exercise
            {
                Id = 1,
                Name = "Push Up",
                Description = "Standard pushup",
                Difficulty = "Beginner"
            };
            _dbContext.Exercises.Add(exercise);

            var workout = new Workout
            {
                Id = 1,
                Name = "Benchmark Workout",
                Description = "Testing performance",
                Category = "Strength",
                Difficulty = "Intermediate",
                DurationInMinutes = 60,
                CaloriesBurn = 500,
                IsPremium = false,
                Rating = 4.5,
                WorkoutPlanId = 1,
                WorkoutExercises = new List<WorkoutExercise>
                {
                    new WorkoutExercise
                    {
                        ExerciseId = 1,
                        Sets = 3,
                        Reps = "10",
                        RestTimeInSeconds = 60,
                        Order = 1
                    }
                }
            };

            _dbContext.Workouts.Add(workout);
            _dbContext.SaveChanges();

            // 3. Initialize Handler using the Real Repository (No Moq)
            // Instead of using Moq (which adds memory overhead for dynamic proxies),
            // we use the actual BaseRepository implementation connected to the InMemory DB.
            var realRepo = new BaseRepository<Workout>(_dbContext);

            _handler = new GetWorkoutDetailsHandler(realRepo);
            _query = new GetWorkoutDetailsQuery(1);
        }

        [Benchmark]
        public async Task GetWorkoutDetails()
        {
            // Execute the handler logic.
            // The overhead here should now reflect only EF Core and your mapping logic.
            await _handler.Handle(_query, CancellationToken.None);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            // Clean up resources to prevent memory leaks after the benchmark completes.
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}