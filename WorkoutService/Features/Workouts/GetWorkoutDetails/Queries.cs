using MediatR;
using WorkoutService.Features.Shared;
using WorkoutService.Features.Workouts.GetWorkoutDetails.ViewModels;

namespace WorkoutService.Features.Workouts.GetWorkoutDetails
{
    public record GetWorkoutDetailsQuery(int Id) : IRequest<RequestResponse<WorkoutDetailsViewModel>>;
}
