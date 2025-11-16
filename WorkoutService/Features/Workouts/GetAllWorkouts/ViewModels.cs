using WorkoutService.Features.Shared;
using WorkoutService.Features.Workouts.CreateWorkout.ViewModels;

namespace WorkoutService.Features.Workouts.GetAllWorkouts.ViewModels
{
    public record PaginatedWorkoutsVm : PaginatedResult<WorkoutVm>
    {
        public PaginatedWorkoutsVm(List<WorkoutVm> items, int count, int pageNumber, int pageSize)
            : base(items, count, pageNumber, pageSize)
        {
        }
    }
}
