using BenchmarkDotNet.Attributes;
using LinqKit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Entities;
using WorkoutService.Features.Workouts.GetAllWorkouts;
using WorkoutService.Infrastructure;
using WorkoutService.Infrastructure.Data;

namespace WorkoutService.Benchmark
{
    [MemoryDiagnoser]
    public class GetAllWorkoutsBenchmark
    {
        private SqliteConnection _connection;
        private DbContextOptions<ApplicationDbContext> _options;
        private GetAllWorkoutsQuery _query;

        [GlobalSetup]
        public void Setup()
        {
            // 1. Setup SQLite In-Memory
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // ✅ FIX: Separated the builder creation and added explicit cast
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .WithExpressionExpanding(); // This extension returns the base builder

            // Force cast the Options to the generic type
            _options = (DbContextOptions<ApplicationDbContext>)builder.Options;

            // 2. Seed Data
            using var context = new ApplicationDbContext(_options);
            context.Database.EnsureCreated();

            // Seed a Plan first (Foreign Key)
            var plan = new WorkoutPlan
            {
                Id = 1,
                Name = "Plan A",
                Description = "Desc",
                Goal = "Strength",
                Difficulty = "All",
                Status = "Active",
                ExternalPlanId = "Ext_1"
            };
            context.WorkoutPlans.Add(plan);

            var workouts = new List<Workout>();
            for (int i = 0; i < 1000; i++)
            {
                workouts.Add(new Workout
                {
                    Id = i + 1,
                    Name = $"Workout {i} - Strength Training",
                    Description = "Description text",
                    Category = i % 2 == 0 ? "Strength" : "Cardio",
                    Difficulty = i % 3 == 0 ? "Beginner" : "Advanced",
                    DurationInMinutes = (i % 2 == 0) ? 30 : 45,
                    CaloriesBurn = 300,
                    IsPremium = false,
                    Rating = 5,
                    WorkoutPlanId = 1
                });
            }
            context.Workouts.AddRange(workouts);
            context.SaveChanges();

            // 3. Setup Query with Filters
            _query = new GetAllWorkoutsQuery
            (
                Page: 1,
                PageSize: 20,
                Search: "Strength",
                Category: "Strength",
                Difficulty: "Beginner",
                Duration: 30
            );
        }

        [Benchmark]
        public async Task GetAllWorkouts()
        {
            // Scoped Context
            using var context = new ApplicationDbContext(_options);
            var repo = new BaseRepository<Workout>(context);
            var handler = new GetAllWorkoutsHandler(repo);

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