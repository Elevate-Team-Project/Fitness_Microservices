using MassTransit;
using WorkoutService.Contracts;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace WorkoutService.Features.Consumers // Suggested Namespace
{
    public class WorkoutCreatedConsumer : IConsumer<IWorkoutCreated>
    {
        private readonly IBaseRepository<Workout> _workoutRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WorkoutCreatedConsumer> _logger;

        // ✅ Inject Repository and UnitOfWork to handle the DB logic
        public WorkoutCreatedConsumer(
            IBaseRepository<Workout> workoutRepository,
            IUnitOfWork unitOfWork,
            ILogger<WorkoutCreatedConsumer> logger)
        {
            _workoutRepository = workoutRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IWorkoutCreated> context)
        {
            var message = context.Message;
            _logger.LogInformation("📥 Received request to create workout: {Name}", message.Name);

            try
            {
                // 1. Map Message to Entity
                // We are reconstructing the entity from the message payload
                var workout = new Workout
                {
                    Name = message.Name,
                    Description = message.Description,
                    Category = message.Category,
                    Difficulty = message.Difficulty,
                    CaloriesBurn = message.CaloriesBurn,
                    DurationInMinutes = message.DurationInMinutes,
                    IsPremium = message.IsPremium,
                    CreatedAt = message.CreatedAt,
                    WorkoutPlanId = message.WorkoutPlanId,

                    // Initialize Rating (Database requires this)
                    Rating = message.Rating,
                    TotalRatings = 0
                };

                // 2. Save to Database (The actual heavy lifting)
                await _workoutRepository.AddAsync(workout);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("✅ Successfully saved Workout '{Name}' to DB. Generated ID: {Id}", workout.Name, workout.Id);
            }
            catch (Exception ex)
            {
                // 🚨 If saving fails, we log it. 
                // Throwing the exception tells MassTransit to move this message to the _error queue (or retry).
                _logger.LogError(ex, "❌ Failed to save workout '{Name}' due to database error.", message.Name);
                throw;
            }
        }
    }
}
