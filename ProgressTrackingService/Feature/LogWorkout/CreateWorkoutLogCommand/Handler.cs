using MediatR;
using ProgressTrackingService.Domain.Entity;
using ProgressTrackingService.Domain.Interfaces;
using ProgressTrackingService.Feature.LogWorkout.CreateWorkoutLogCommand.DTOs;
using ProgressTrackingService.Feature.LogWorkout.PlaceWorkoutOrchestrator.DTos;

namespace ProgressTrackingService.Feature.LogWorkout.CreateWorkoutLogCommand
{
    public class CommandHandler : IRequestHandler<CreateWorkOutLogCommand, WorkoutLogResponseDto>
    {
        private readonly IUniteOfWork _uow;
        private readonly IGenericRepository<WorkoutLog> _repository;

        public CommandHandler(IUniteOfWork uow , IGenericRepository<WorkoutLog> repository)
        {
            _uow = uow;
            _repository = repository;
        }

        public async Task<WorkoutLogResponseDto> Handle(CreateWorkOutLogCommand request, CancellationToken cancellationToken)
        {
            var workoutLog = new WorkoutLog
            {
                WorkoutId = request.WorkoutLogDto.WorkoutId,
                CompletedAt = request.WorkoutLogDto.CompletedAt,
                Duration = request.WorkoutLogDto.Duration,
                CaloriesBurned = request.WorkoutLogDto.CaloriesBurned,
                UserId = request.WorkoutLogDto.UserId,
                Notes= request.WorkoutLogDto.Notes,
                Rating= request.WorkoutLogDto.Rating,
                SessionId= request.WorkoutLogDto.SessionId,
                WorkoutName= request.WorkoutLogDto.WorkoutName,
                CreatedAt= DateTime.UtcNow
            };
            var workout = await _repository.AddAsync(workoutLog);
            await _uow.SaveChangesAsync();
            var response = new WorkoutLogResponseDto 
            {
             Id = workout.Id,
             WorkoutName=workoutLog.WorkoutName,
             WorkoutId=workoutLog.WorkoutId,
                CompletedAt=workoutLog.CompletedAt,
                Duration=workoutLog.Duration,
                CaloriesBurned=workoutLog.CaloriesBurned,
                CurrentStreak=0,
                TotalWorkouts=0,
                TotalCaloriesBurned=0,
                
                NewAchievements=new List<AchievementDto>(),  
            };
            return response;


        }
    }
}
