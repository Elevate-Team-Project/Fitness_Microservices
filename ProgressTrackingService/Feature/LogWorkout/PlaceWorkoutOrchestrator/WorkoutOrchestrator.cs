using MediatR;
using ProgressTrackingService.Feature.LogWorkout.CreateWorkoutLogCommand;
using ProgressTrackingService.Feature.LogWorkout.PlaceWorkoutOrchestrator.DTos;
using ProgressTrackingService.Feature.UserStatisticsfiles.GetByUserIdQuery;
using ProgressTrackingService.Feature.UserStatisticsfiles.GetUserstatisticsByIdQuery;
using ProgressTrackingService.Feature.UserStatisticsfiles.UpdateUserStatistics;

namespace ProgressTrackingService.Feature.LogWorkout.PlaceWorkoutOrchestrator
{
    public record WorkoutOrchestrator(WorkoutLogDto WorkoutLog) : IRequest<WorkoutLogResponseDto>;

    public class WorkoutOrchestratorHandler : IRequestHandler<WorkoutOrchestrator, WorkoutLogResponseDto>
    {
        private readonly IMediator _mediator;
        public WorkoutOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<WorkoutLogResponseDto> Handle(WorkoutOrchestrator request, CancellationToken cancellationToken)
        {
            var createWorkoutLogCommand = new CreateWorkOutLogCommand (request.WorkoutLog);
            var workoutLogResponse = await _mediator.Send(createWorkoutLogCommand, cancellationToken);
            // Additional orchestration logic can be added here in the future
            var UpdateUserStatsCommand = new UpdateUserstatisticsCommand(request.WorkoutLog.CaloriesBurned,request.WorkoutLog.UserId);
           await _mediator.Send(UpdateUserStatsCommand, cancellationToken);
            
            var userStatisticsId = await _mediator.Send(new GetUserStatisticsId_ByUserIdQuery(request.WorkoutLog.UserId), cancellationToken);

            var userstatistics = await _mediator.Send(new GetUserStatisticsQuery(userStatisticsId), cancellationToken);
            var response = new WorkoutLogResponseDto 
            { 
             Id = workoutLogResponse.Id,
                WorkoutId = workoutLogResponse.WorkoutId,
                WorkoutName = workoutLogResponse.WorkoutName,
                CompletedAt = workoutLogResponse.CompletedAt,
                Duration = workoutLogResponse.Duration,
                CaloriesBurned = workoutLogResponse.CaloriesBurned,
                CurrentStreak = userstatistics.CurrentStreak,
                TotalWorkouts = userstatistics.TotalWorkouts,
                TotalCaloriesBurned = userstatistics.TotalCaloriesBurned,
                NewAchievements = workoutLogResponse.NewAchievements
                

            };
            return response;

        }
    } 

}
