using MassTransit;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Contracts;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Interfaces;

namespace WorkoutService.Features.Consumers
{
    public class WorkoutSessionStartedConsumer : IConsumer<IWorkoutSessionStarted>
    {
        private readonly IBaseRepository<Workout> _workoutRepository;
        private readonly IBaseRepository<WorkoutSession> _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WorkoutSessionStartedConsumer> _logger;

        public WorkoutSessionStartedConsumer(
            IBaseRepository<Workout> workoutRepository,
            IBaseRepository<WorkoutSession> sessionRepository,
            IUnitOfWork unitOfWork,
            ILogger<WorkoutSessionStartedConsumer> logger)
        {
            _workoutRepository = workoutRepository;
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IWorkoutSessionStarted> context)
        {
            var message = context.Message;
            _logger.LogInformation("🚀 Processing Start Session for Workout ID: {WorkoutId}", message.WorkoutId);

            try
            {
                // 1. Fetch the Workout Data (We need the list of exercises to copy them)
                // We use AsNoTracking because we are only reading the template data.
                var workout = await _workoutRepository.GetAll()
                    .Include(w => w.WorkoutExercises)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(w => w.Id == message.WorkoutId);

                if (workout == null)
                {
                    _logger.LogError("❌ Workout {WorkoutId} not found. Session creation aborted.", message.WorkoutId);
                    return; // Message is consumed but nothing happens (or throw to retry)
                }

                // 2. Create the Session Entity
                var session = new WorkoutSession
                {
                    WorkoutId = message.WorkoutId,
                    UserId = message.UserId,
                    Status = "InProgress",
                    StartedAt = message.StartedAt, // Use the time from the message for consistency
                    PlannedDurationInMinutes = message.PlannedDurationMinutes,
                    Difficulty = message.Difficulty,

                    // 3. Map Workout Exercises to Session Exercises
                    // We copy the structure so the user can track progress on each exercise individually.
                    SessionExercises = workout.WorkoutExercises.Select(we => new WorkoutSessionExercise
                    {
                        ExerciseId = we.ExerciseId,
                        Order = we.Order,
                        Status = "Pending", // Default status for new session
                        Sets = we.Sets,
                        Reps = we.Reps,
                        RestTimeInSeconds = we.RestTimeInSeconds
                    }).ToList()
                };

                // 4. Save to Database
                await _sessionRepository.AddAsync(session);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("✅ Session successfully created with ID: {SessionId}", session.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to create session for Workout {WorkoutId}", message.WorkoutId);
                throw; // Allows MassTransit to retry or move to _error queue
            }
        }
    }
}