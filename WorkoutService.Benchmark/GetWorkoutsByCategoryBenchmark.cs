using BenchmarkDotNet.Attributes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Entities;
using WorkoutService.Features.Workouts.GetWorkoutsByCategory;
using WorkoutService.Infrastructure;
using WorkoutService.Infrastructure.Data;

namespace WorkoutService.Benchmark
{
    [MemoryDiagnoser]
    public class GetWorkoutsByCategoryBenchmark
    {
        private SqliteConnection _connection;
        private DbContextOptions<ApplicationDbContext> _options;
        private GetWorkoutsByCategoryQuery _query;

        [GlobalSetup]
        public void Setup()
        {
            // 1. Setup SQLite Connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // 2. Setup Options
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            // 3. Seed Data
            using var context = new ApplicationDbContext(_options);
            context.Database.EnsureCreated();

            // ✅ Fix: Must seed a WorkoutPlan first (Foreign Key Constraint)
            var plan = new WorkoutPlan
            {
                Id = 1,
                Name = "Benchmark Plan",
                Description = "Desc",
                Goal = "Strength",
                Difficulty = "All",
                Status = "Active",
                ExternalPlanId = "EXT_1"
            };
            context.WorkoutPlans.Add(plan);

            var workouts = new List<Workout>();
            for (int i = 0; i < 1000; i++)
            {
                workouts.Add(new Workout
                {
                    Id = i + 1,
                    Name = $"Workout {i}",
                    Description = "Benchmark description text",
                    Category = i % 2 == 0 ? "Strength" : "Cardio",
                    Difficulty = i % 3 == 0 ? "Beginner" : "Advanced",
                    DurationInMinutes = 30,
                    CaloriesBurn = 200,
                    IsPremium = false,
                    Rating = 5,
                    WorkoutPlanId = 1 // ✅ Link to the created Plan
                });
            }
            context.Workouts.AddRange(workouts);
            context.SaveChanges();

            // 4. Setup Query
            _query = new GetWorkoutsByCategoryQuery("Strength", 1, 20, "Advanced");
        }

        [Benchmark]
        public async Task GetWorkoutsByCategory()
        {
            using var context = new ApplicationDbContext(_options);
            var repo = new BaseRepository<Workout>(context);
            var handler = new GetWorkoutsByCategoryHandler(repo);

            await handler.Handle(_query, CancellationToken.None);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}