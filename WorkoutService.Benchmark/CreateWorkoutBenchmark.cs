using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
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

        // ---------------------------------------------------------
        // 1. Safe UnitOfWork Implementation
        // ---------------------------------------------------------
        // We create a private implementation to avoid using Moq (which adds overhead).
        private class BenchmarkUnitOfWork : IUnitOfWork
        {
            private readonly ApplicationDbContext _context;

            public BenchmarkUnitOfWork(ApplicationDbContext context) => _context = context;

            // This is the method actually used by the Handler
            public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
                => await _context.SaveChangesAsync(cancellationToken);

            // Safe implementations for unused methods (to prevent exceptions if logic changes)
            public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
            public void Dispose() { }
            public Task BeginTransactionAsync() => Task.CompletedTask;    // Simulates starting a transaction
            public Task CommitTransactionAsync() => Task.CompletedTask;   // Simulates committing
            public Task RollbackTransactionAsync() => Task.CompletedTask; // Simulates rollback
        }

        [GlobalSetup]
        public void Setup()
        {
            // 2. Configure Options Only
            // We do NOT create the context instance here. We only configure the options
            // to use a shared InMemory database.
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "BenchmarkDb_" + Guid.NewGuid().ToString())
                .Options;

            // 3. Prepare the Command (Data to be inserted)
            // We create this once to exclude object creation cost from the benchmark.
            var dto = new CreateWorkoutDto("New Workout", "Description for benchmark");
            _command = new CreateWorkoutCommand(dto);
        }

        [Benchmark]
        public async Task CreateWorkout()
        {
            // -------------------------------------------------------------
            // CRITICAL FIX: Simulate Request Scope
            // -------------------------------------------------------------
            // In a real API, a new DbContext is created for every request.
            // By creating it inside the [Benchmark] method, we ensure the 
            // ChangeTracker starts empty every time.
            // This prevents the "17MB Allocation" issue caused by tracking thousands of objects.

            using var context = new ApplicationDbContext(_options);

            // Initialize dependencies with the fresh context
            var unitOfWork = new BenchmarkUnitOfWork(context);
            var repo = new BaseRepository<Workout>(context);
            //var handler = new CreateWorkoutHandler(unitOfWork, repo);

            //// Execute the handler
            //await handler.Handle(_command, CancellationToken.None);

            // 'using' statement disposes the context automatically here,
            // clearing the tracked memory.
        }
    }
}