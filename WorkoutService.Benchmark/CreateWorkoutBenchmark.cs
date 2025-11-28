using BenchmarkDotNet.Attributes;
using LinqKit;
using MassTransit;
using Microsoft.Data.Sqlite; // ✅ Using SQLite for accurate relational simulation
using Microsoft.EntityFrameworkCore;
using Moq; // ✅ Mocking MassTransit
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
        private SqliteConnection _connection;
        private DbContextOptions<ApplicationDbContext> _options;
        private CreateWorkoutCommand _command;
        private IPublishEndpoint _mockPublishEndpoint;

        // ---------------------------------------------------------
        // 1. Lightweight UnitOfWork Implementation for Benchmark
        // ---------------------------------------------------------
        private class BenchmarkUnitOfWork : IUnitOfWork
        {
            private readonly ApplicationDbContext _context;
            public BenchmarkUnitOfWork(ApplicationDbContext context) => _context = context;

            public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
                => await _context.SaveChangesAsync(cancellationToken);

            public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
            public void Dispose() { }

            // No-op for transaction methods as we rely on EF Core's implicit transaction for SaveChanges
            public Task BeginTransactionAsync() => Task.CompletedTask;
            public Task CommitTransactionAsync() => Task.CompletedTask;
            public Task RollbackTransactionAsync() => Task.CompletedTask;
        }

        [GlobalSetup]
        public void Setup()
        {
            // 2. Setup SQLite In-Memory Connection
            // We keep the connection open to persist the schema across benchmark iterations
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // 3. Configure EF Core to use SQLite & Enable LinqKit (for consistency)
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .WithExpressionExpanding(); // Ensure this extension is available

            _options = (DbContextOptions<ApplicationDbContext>)builder.Options;

            // 4. Seed Essential Data (To satisfy Foreign Key constraints)
            using var context = new ApplicationDbContext(_options);
            context.Database.EnsureCreated(); // Creates Outbox tables + Domain tables

            // Seed a WorkoutPlan so the Create Handler doesn't fail on FK
            if (!context.WorkoutPlans.Any())
            {
                context.WorkoutPlans.Add(new WorkoutPlan
                {
                    Id = 1, // Explicit ID for seeding reference
                    Name = "Benchmark Plan",
                    ExternalPlanId = "BENCH_01",
                    Goal = "Testing",
                    Difficulty = "All",
                    Status = "Active"
                });
                context.SaveChanges();
            }

            // 5. Setup the Command Payload
            var dto = new CreateWorkoutDto
            (
                Name: "Benchmark Workout",
                Description: "Testing Outbox Performance",
                CaloriesBurn: 300,
                IsPremium: true,
                Rating: 4.5,
                DurationInMinutes: 45,
                Difficulty: "Intermediate",
                Category: "Strength",
                workoutPlanId: 1 // Must match the seeded Plan ID
            );
            _command = new CreateWorkoutCommand(dto);

            // 6. Mock MassTransit
            // We just need Publish to return a completed task.
            // The actual "Outbox" saving happens in SaveChanges(), handled by EF Core.
            var mock = new Mock<IPublishEndpoint>();
            mock.Setup(x => x.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockPublishEndpoint = mock.Object;
        }

        [Benchmark]
        public async Task CreateWorkout()
        {
            // -------------------------------------------------------------
            // Scoped Execution: Simulate a full HTTP Request lifecycle
            // -------------------------------------------------------------
            using var context = new ApplicationDbContext(_options);

            var unitOfWork = new BenchmarkUnitOfWork(context);
            var repo = new BaseRepository<Workout>(context);

            // Inject the Mock Publisher
            var handler = new CreateWorkoutHandler(_mockPublishEndpoint, unitOfWork);

            // Execute Logic:
            // 1. Publish (Adds to Outbox entity in memory)
            // 2. AddAsync (Adds Workout entity in memory)
            // 3. SaveChanges (Writes BOTH to SQLite in one transaction)
            await handler.Handle(_command, CancellationToken.None);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}