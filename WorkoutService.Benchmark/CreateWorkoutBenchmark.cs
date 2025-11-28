using BenchmarkDotNet.Attributes;
using MassTransit; // ✅ Required for IPublishEndpoint
using Microsoft.EntityFrameworkCore;
using Moq; // ✅ Required for Mocking
using WorkoutService.Contracts;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Interfaces;
using WorkoutService.Features.Workouts.CreateWorkout;
using WorkoutService.Features.Workouts.CreateWorkout.ViewModels;
using WorkoutService.Infrastructure;
using WorkoutService.Infrastructure.Data;

namespace WorkoutService.Benchmark
{
    [MemoryDiagnoser]
    public class CreateWorkoutBenchmark
    {
        private DbContextOptions<ApplicationDbContext> _options;
        private CreateWorkoutCommand _command;
        private IPublishEndpoint _mockPublishEndpoint; // ✅ We will reuse this mock

        // ---------------------------------------------------------
        // 1. Safe UnitOfWork Implementation
        // ---------------------------------------------------------
        private class BenchmarkUnitOfWork : IUnitOfWork
        {
            private readonly ApplicationDbContext _context;
            public BenchmarkUnitOfWork(ApplicationDbContext context) => _context = context;

            public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
                => await _context.SaveChangesAsync(cancellationToken);

            public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
            public void Dispose() { }
            public Task BeginTransactionAsync() => Task.CompletedTask;
            public Task CommitTransactionAsync() => Task.CompletedTask;
            public Task RollbackTransactionAsync() => Task.CompletedTask;
        }

        [GlobalSetup]
        public void Setup()
        {
            // 2. Configure Options (Using InMemory for speed in this specific write test)
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "BenchmarkDb_" + Guid.NewGuid())
                .Options;

            // 3. Seed Required Data (Critical for Foreign Key Constraints)
            using (var context = new ApplicationDbContext(_options))
            {
                // We must add a WorkoutPlan because CreateWorkoutHandler now validates the PlanId
                if (!context.WorkoutPlans.Any())
                {
                    context.WorkoutPlans.Add(new WorkoutPlan
                    {
                        Id = 1, // We will use this ID in the command
                        Name = "Default Plan",
                        Description = "For Benchmark",
                        Goal = "Strength",
                        Difficulty = "Beginner",
                        Status = "Active",
                        ExternalPlanId = "EXT_01"
                    });
                    context.SaveChanges();
                }
            }

            // 4. Prepare the Command
            var dto = new CreateWorkoutDto
            (
                Name: "New Workout",
                Description: "Description for benchmark",
                CaloriesBurn: 200,
                Category: "Strength",
                Difficulty: "Beginner",
                DurationInMinutes: 30,
                IsPremium: false,
                Rating: 4.5,
                workoutPlanId: 1 // ✅ Must match the seeded Plan ID
            );
            _command = new CreateWorkoutCommand(dto);

            // 5. Setup Mock for MassTransit
            // We create a fake publisher that does nothing (Task.CompletedTask)
            var mock = new Mock<IPublishEndpoint>();
            mock.Setup(x => x.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockPublishEndpoint = mock.Object;
        }

        [Benchmark]
        public async Task CreateWorkout()
        {
            // -------------------------------------------------------------
            // Simulate Request Scope (Fresh Context per iteration)
            // -------------------------------------------------------------
            using var context = new ApplicationDbContext(_options);

            var unitOfWork = new BenchmarkUnitOfWork(context);
            var repo = new BaseRepository<Workout>(context);

            // ✅ Inject the Mock Publisher
            var handler = new CreateWorkoutHandler(_mockPublishEndpoint);

            // Execute
            await handler.Handle(_command, CancellationToken.None);
        }
    }
}