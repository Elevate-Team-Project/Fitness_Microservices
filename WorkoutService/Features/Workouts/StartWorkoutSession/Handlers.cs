using Mapster;
using MediatR;
using MassTransit; // ✅ Required
using WorkoutService.Contracts; // ✅ Required
using WorkoutService.Features.Shared;
using WorkoutService.Features.Workouts.StartWorkoutSession.ViewModels;

namespace WorkoutService.Features.Workouts.StartWorkoutSession
{
    public class StartWorkoutSessionCommandHandler : IRequestHandler<StartWorkoutSessionCommand, RequestResponse<WorkoutSessionViewModel>>
    {
        private readonly IPublishEndpoint _publishEndpoint;

        // ✅ Lightweight Constructor: No Repositories, only MassTransit
        public StartWorkoutSessionCommandHandler(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task<RequestResponse<WorkoutSessionViewModel>> Handle(StartWorkoutSessionCommand request, CancellationToken cancellationToken)
        {
            // 1. Prepare Data
            var startedAt = DateTime.UtcNow;

            // Mocking UserID (In a real app, extract this from the HTTP Context/Token)
            var userId = Guid.NewGuid();

            // 2. Publish "Fire-and-Forget" Event
            // The Consumer will handle fetching details and saving to the DB.
            await _publishEndpoint.Publish<IWorkoutSessionStarted>(new
            {
                WorkoutId = request.WorkoutId,
                UserId = userId,
                PlannedDurationMinutes = request.Dto.PlannedDuration,
                Difficulty = request.Dto.Difficulty,
                StartedAt = startedAt
            }, cancellationToken);

            // 3. Return Immediate Provisional Response
            // Since we are not querying the DB, we cannot return 'WorkoutName' or 'Exercises' yet.
            // The Frontend should handle this state (e.g., show a spinner or use cached data).
            var responseVm = new WorkoutSessionViewModel
            {
                SessionId = "0", // Indicates "Pending Creation"
                WorkoutId = request.WorkoutId,
                WorkoutName = "Processing...", // Placeholder as we didn't fetch it
                status = "InProgress",
                PlannedDuration = request.Dto.PlannedDuration,
                Difficulty = request.Dto.Difficulty,
                StartedAt = startedAt,
                Exercises = new List<SessionExerciseViewModel>() // Empty list initially
            };

            return RequestResponse<WorkoutSessionViewModel>.Success(responseVm, "Workout session start request queued successfully.");
        }
    }
}