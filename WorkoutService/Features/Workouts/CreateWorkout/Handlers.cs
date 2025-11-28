using MediatR;
using MassTransit;
using WorkoutService.Features.Workouts.CreateWorkout.ViewModels;
using WorkoutService.Contracts;

namespace WorkoutService.Features.Workouts.CreateWorkout
{
    public class CreateWorkoutHandler : IRequestHandler<CreateWorkoutCommand, WorkoutVm>
    {
        private readonly IPublishEndpoint _publishEndpoint;

        // ✅ We only need IPublishEndpoint. No Repository/UnitOfWork needed here.
        public CreateWorkoutHandler(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task<WorkoutVm> Handle(CreateWorkoutCommand request, CancellationToken cancellationToken)
        {
            // 1. Prepare the payload
            // Note: ID will be 0 here because DB hasn't generated it yet.
            // If you need an ID reference, consider generating a Guid 'CorrelationId' to track this request.

            await _publishEndpoint.Publish<IWorkoutCreated>(new
            {
                // We send ALL data required for saving
                WorkoutId = 0,
                Name = request.Dto.Name,
                Description = request.Dto.Description,
                CaloriesBurn = request.Dto.CaloriesBurn,
                Category = request.Dto.Category,
                Difficulty = request.Dto.Difficulty,
                DurationInMinutes = request.Dto.DurationInMinutes,
                IsPremium = request.Dto.IsPremium,
                Rating = 0.0,
                CreatedAt = DateTime.UtcNow,

                // IMPORTANT: You MUST pass the PlanId so the Consumer can link it
                WorkoutPlanId = request.Dto.workoutPlanId
            }, cancellationToken);

            // 2. Return Response
            // Warning: The returned ID will be 0. The client must be aware of this.
            return new WorkoutVm(0, request.Dto.Name, request.Dto.Description);
        }
    }
}