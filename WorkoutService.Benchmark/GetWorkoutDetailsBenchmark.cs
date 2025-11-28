using BenchmarkDotNet.Attributes;
using Microsoft.Data.Sqlite; // ✅ Required
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
        private SqliteConnection _connection;
        private ApplicationDbContext _dbContext;
        private GetWorkoutDetailsHandler _handler;
        private GetWorkoutDetailsQuery _query;

        [GlobalSetup]
        public void Setup()
        {
            // 1. Setup SQLite In-Memory Connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open(); // Keep connection open

            // 2. Configure Options
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _dbContext.Database.EnsureCreated(); // Create tables

            // 3. Seed Data
            // Create Plan first (Foreign Key)
            var workoutPlan = new WorkoutPlan
            {
                Id = 1,
                Name = "Test Plan",
                Description = "Desc",
                Goal = "Strength",
                Difficulty = "Intermediate",
                Status = "Active",
                ExternalPlanId = "plan_123"
            };
            _dbContext.WorkoutPlans.Add(workoutPlan);

            // Create Exercise
            var exercise = new Exercise
            {
                Id = 1,
                Name = "Push Up",
                Description = "Standard pushup",
                Difficulty = "Beginner"
            };
            _dbContext.Exercises.Add(exercise);

            // Create Workout
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

            // 4. Initialize Handler
            var repo = new BaseRepository<Workout>(_dbContext);
            _handler = new GetWorkoutDetailsHandler(repo);
            _query = new GetWorkoutDetailsQuery(1);
        }

        [Benchmark]
        public async Task GetWorkoutDetails()
        {
            // Using the existing context (Unscoped) to measure query speed purely
            await _handler.Handle(_query, CancellationToken.None);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _connection.Close();
            _connection.Dispose();
            _dbContext.Dispose();
        }
    }
}