using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Moq;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Interfaces;
using WorkoutService.Features.Workouts.GetWorkoutDetails;
using WorkoutService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.InMemory;

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
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each run
                .Options;

            _dbContext = new ApplicationDbContext(options);

            // Seed data
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

            // Mock Repository
            var mockRepo = new Mock<IBaseRepository<Workout>>();
            // We need to ensure GetAll returns the DbSet so EF Core functionality (Select) works
            mockRepo.Setup(r => r.GetAll()).Returns(_dbContext.Workouts);

            _handler = new GetWorkoutDetailsHandler(mockRepo.Object);
            _query = new GetWorkoutDetailsQuery(1);
        }

        [Benchmark]
        public async Task GetWorkoutDetails()
        {
            await _handler.Handle(_query, CancellationToken.None);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
