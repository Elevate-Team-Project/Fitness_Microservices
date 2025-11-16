using MediatR;
using WorkoutService.Features.Shared;
using WorkoutService.Features.Workouts.GetAllWorkouts.ViewModels;

namespace WorkoutService.Features.Workouts.GetWorkoutsByCategory
{
    public record GetWorkoutsByCategoryQuery(
        string CategoryName,
        int Page = 1,
        int PageSize = 20,
        string? Difficulty = null
    ) : IRequest<RequestResponse<PaginatedResult<WorkoutViewModel>>>;
}
