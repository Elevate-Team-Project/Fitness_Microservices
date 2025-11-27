using BenchmarkDotNet.Attributes;
using Mapster;
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
        private DbContextOptions<ApplicationDbContext> _options;
        private GetWorkoutsByCategoryQuery _query;

        [GlobalSetup]
        public void Setup()
        {
            // 1. Setup Options
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ReadBench_" + Guid.NewGuid())
                .Options;

            // 2. Seed Data (مرة واحدة في البداية)
            using var context = new ApplicationDbContext(_options);
            var workouts = new List<Workout>();
            for (int i = 0; i < 1000; i++)
            {
                workouts.Add(new Workout
                {
                    Id = i + 1,
                    Name = $"Workout {i}",
                    Category = i % 2 == 0 ? "Strength" : "Cardio",
                    Difficulty = i % 3 == 0 ? "Beginner" : "Advanced",
                    Rating = 5
                    // ... باقي الداتا
                });
            }
            context.Workouts.AddRange(workouts);
            context.SaveChanges();

            // 3. Setup Query
            _query = new GetWorkoutsByCategoryQuery("Strength", 1, 20, "Advanced");
        }

        [Benchmark]
        public async Task GetWorkoutsByCategory()
        {
            // Scoped Context: إنشاء كونتكست جديد لكل لفة
            using var context = new ApplicationDbContext(_options);
            var repo = new BaseRepository<Workout>(context);
            var handler = new GetWorkoutsByCategoryHandler(repo);

            await handler.Handle(_query, CancellationToken.None);
        }
    }
}