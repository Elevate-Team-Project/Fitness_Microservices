using MediatR;
using MassTransit; // ✅ 1. Add MassTransit namespace
using WorkoutService.Features.Workouts.CreateWorkout.ViewModels;
using WorkoutService.Domain.Interfaces;
using WorkoutService.Domain.Entities;
using WorkoutService.Contracts; // ✅ 2. Add Contracts namespace (Where IWorkoutCreated is defined)

namespace WorkoutService.Features.Workouts.CreateWorkout
{
    public class CreateWorkoutHandler : IRequestHandler<CreateWorkoutCommand, WorkoutVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<Workout> _workoutRepository;
        private readonly IPublishEndpoint _publishEndpoint; // ✅ 3. Add Publish Endpoint field

        // ✅ 4. Inject IPublishEndpoint in Constructor
        public CreateWorkoutHandler(
            IUnitOfWork unitOfWork,
            IBaseRepository<Workout> workoutRepository,
            IPublishEndpoint publishEndpoint)
        {
            _unitOfWork = unitOfWork;
            _workoutRepository = workoutRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<WorkoutVm> Handle(CreateWorkoutCommand request, CancellationToken cancellationToken)
        {
            // TODO: Add mapping (Consider using Mapster here later)
            var workout = new Workout
            {
                Name = request.Dto.Name,
                Description = request.Dto.Description,
                CaloriesBurn = request.Dto.CaloriesBurn,
                Category = request.Dto.Category,
                Difficulty = request.Dto.Difficulty,
                DurationInMinutes = request.Dto.DurationInMinutes,
                IsPremium = request.Dto.IsPremium,
                Rating = 0.0,
                CreatedAt = DateTime.UtcNow // Assuming you have this field
            };
            // ✅ 5. Publish the Event to RabbitMQ
            // We use an anonymous object that matches the IWorkoutCreated interface properties.
            await _publishEndpoint.Publish<IWorkoutCreated>(new
            {
                WorkoutId = workout.Id,
                Name = workout.Name,
                Description = workout.Description,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);

            await _workoutRepository.AddAsync(workout);
            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return new WorkoutVm(workout.Id, workout.Name, workout.Description);
        }
    }
}