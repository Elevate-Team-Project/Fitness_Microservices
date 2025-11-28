using Mapster;
using MediatR;
using MassTransit; // ✅ Required for Messaging
using WorkoutService.Contracts; // ✅ Required for Contracts
using WorkoutService.Features.Shared;
using WorkoutService.Features.Workouts.StartWorkoutSession.ViewModels;
using WorkoutService.Domain.Interfaces; // ✅ Required for ICurrentUserService

namespace WorkoutService.Features.Workouts.StartWorkoutSession
{
    public class StartWorkoutSessionCommandHandler : IRequestHandler<StartWorkoutSessionCommand, RequestResponse<WorkoutSessionViewModel>>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ICurrentUserService _currentUserService; // ✅ 1. Inject CurrentUserService

        // ✅ Lightweight Constructor: MassTransit + User Service only
        public StartWorkoutSessionCommandHandler(
            IPublishEndpoint publishEndpoint,
            ICurrentUserService currentUserService)
        {
            _publishEndpoint = publishEndpoint;
            _currentUserService = currentUserService;
        }

        public async Task<RequestResponse<WorkoutSessionViewModel>> Handle(StartWorkoutSessionCommand request, CancellationToken cancellationToken)
        {
            // ✅ 2. Security Check: Validate User is Authenticated
            if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
            {
                // 401 Unauthorized
                return RequestResponse<WorkoutSessionViewModel>.Fail("User is not authenticated");
            }

            // ✅ 3. Parse User ID from Token (Assuming Auth Service provides Guid)
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                // 400 Bad Request if ID format is wrong
                return RequestResponse<WorkoutSessionViewModel>.Fail("Invalid User ID format in token");
            }

            // 4. Prepare Data
            var startedAt = DateTime.UtcNow;

            // 5. Publish "Fire-and-Forget" Event with REAL User ID
            await _publishEndpoint.Publish<IWorkoutSessionStarted>(new
            {
                WorkoutId = request.WorkoutId,
                UserId = userId, // ✅ Using the real User ID from Token
                PlannedDurationMinutes = request.Dto.PlannedDuration,
                Difficulty = request.Dto.Difficulty,
                StartedAt = startedAt
            }, cancellationToken);

            // 6. Return Provisional Response
            var responseVm = new WorkoutSessionViewModel
            {
                SessionId = "0", // Indicates Pending Creation
                WorkoutId = request.WorkoutId,
                WorkoutName = "Processing...", // Placeholder
                status = "InProgress",
                PlannedDuration = request.Dto.PlannedDuration,
                Difficulty = request.Dto.Difficulty,
                StartedAt = startedAt,
                Exercises = new List<SessionExerciseViewModel>()
            };

            return RequestResponse<WorkoutSessionViewModel>.Success(responseVm, "Workout session start request queued successfully.");
        }
    }
}