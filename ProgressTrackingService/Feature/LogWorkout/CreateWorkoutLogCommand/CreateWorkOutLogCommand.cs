using MediatR;
using ProgressTrackingService.Feature.LogWorkout.PlaceWorkoutOrchestrator.DTos;

namespace ProgressTrackingService.Feature.LogWorkout.CreateWorkoutLogCommand
{
    public record CreateWorkOutLogCommand(WorkoutLogDto WorkoutLogDto) : IRequest<WorkoutLogResponseDto>;
    
}
