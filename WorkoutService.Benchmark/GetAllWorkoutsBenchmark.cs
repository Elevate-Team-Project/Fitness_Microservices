using BenchmarkDotNet.Attributes;
using LinqKit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory; // ✅ Required for Cache
using Microsoft.Extensions.DependencyInjection; // ✅ Required for DI & Pooling
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
        private IServiceProvider _serviceProvider; // ✅ Container to manage Pooling & Cache
        private GetAllWorkoutsQuery _query;

        [GlobalSetup]
        public void Setup()
        {
            // 1. Setup SQLite In-Memory Connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // 2. Setup Dependency Injection (DI) Container
            // This allows us to use DbContextPool and MemoryCache exactly like the real app.
            var services = new ServiceCollection();

            // Add Memory Cache
            services.AddMemoryCache();

            // Add DbContext with Pooling & LinqKit & SQLite
            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                options.UseSqlite(_connection)
                       .WithExpressionExpanding(); // ✅ LinqKit Support
            });

            _serviceProvider = services.BuildServiceProvider();

            // 3. Seed Data (Using a temporary scope)
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();

                // Seed Plan
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

                // Seed Workouts (1000 items)
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
            }

            // 4. Setup Query
            _query = new GetAllWorkoutsQuery(1, 20, "Strength", "Beginner", 30);
        }

        [Benchmark]
        public async Task GetAllWorkouts()
        {
            // ✅ Scoped Execution with Pooling
            // We ask the ServiceProvider for a new Scope.
            // The DbContext inside this scope comes from the Pool (Recycled), 
            // drastically reducing Gen0 allocations.
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

                var repo = new BaseRepository<Workout>(context);

                // Inject both Repo and Cache
                var handler = new GetAllWorkoutsHandler(repo, cache);

                await handler.Handle(_query, CancellationToken.None);
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _connection.Close();
            _connection.Dispose();
            if (_serviceProvider is IDisposable d) d.Dispose();
        }
    }
}