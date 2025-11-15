using MediatR;
using WorkoutService.Features.Workouts.GetAllWorkouts.ViewModels;

namespace WorkoutService.Features.Workouts.GetAllWorkouts
{
    public class GetAllWorkoutsQuery : IRequest<PaginatedWorkoutsVm>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Category { get; set; }
        public string? Difficulty { get; set; }
        public int? Duration { get; set; }
        public string? Search { get; set; }
    }
}
