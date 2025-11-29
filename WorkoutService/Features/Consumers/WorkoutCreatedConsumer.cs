using MassTransit;
using WorkoutService.Contracts;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory; // ✅ Required for IMemoryCache

namespace WorkoutService.Features.Consumers
{
    public class WorkoutCreatedConsumer : IConsumer<IWorkoutCreated>
    {
        private readonly IBaseRepository<Workout> _workoutRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WorkoutCreatedConsumer> _logger;
        private readonly IMemoryCache _cache; // ✅ Inject Cache Service

        public WorkoutCreatedConsumer(
            IBaseRepository<Workout> workoutRepository,
            IUnitOfWork unitOfWork,
            ILogger<WorkoutCreatedConsumer> logger,
            IMemoryCache cache)
        {
            _workoutRepository = workoutRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cache = cache;
        }

        public async Task Consume(ConsumeContext<IWorkoutCreated> context)
        {
            var message = context.Message;
            _logger.LogInformation("📥 Received request to create workout: {Name}", message.Name);

            try
            {
                // 1. Map Message to Entity
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
                    Rating = message.Rating,
                    TotalRatings = 0
                };

                // 2. Save to Database
                await _workoutRepository.AddAsync(workout);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("✅ Successfully saved Workout '{Name}' to DB. Generated ID: {Id}", workout.Name, workout.Id);

                // 3. Invalidate Cache (The Fix)
                // Since a new workout is added, the cached list of workouts is now stale.
                // We need to clear it so the next 'GetAll' request fetches fresh data from the DB.

                // Note: IMemoryCache does not support wildcard removal (e.g., "Workouts_*").
                // However, for a MemoryCache implementation, we can use compacting or specific keys if known.
                // In a distributed cache (Redis), we would use tags or a set.
                // For simplicity and effectiveness here, we can clear specific known keys or rely on short expiration.
                // A common pattern for MemoryCache in simple scenarios is to use a "Token" for dependencies.

                // Assuming we don't have a complex tagging system yet, we rely on the short sliding expiration (2 mins).
                // BUT, if immediate consistency is required, we could use CancellationTokenSource or just accept
                // eventual consistency (which is fine for CQRS).

                // IF you want to force clear (Brute force for MemoryCache):
                if (_cache is MemoryCache concreteCache)
                {
                    // This clears 100% of the cache. It's heavy but guarantees freshness.
                    // Use with caution in high-load production apps.
                    concreteCache.Compact(100);
                    _logger.LogInformation("🧹 Cache cleared (Compacted) to ensure data freshness.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to save workout '{Name}' due to database error.", message.Name);
                throw;
            }
        }
    }
}