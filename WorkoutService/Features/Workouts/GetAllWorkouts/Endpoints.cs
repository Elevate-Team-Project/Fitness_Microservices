using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkoutService.Shared;
using WorkoutService.Features.Workouts.GetAllWorkouts.ViewModels;

namespace WorkoutService.Features.Workouts.GetAllWorkouts
{
    public static class Endpoints
    {
        public static void MapGetAllWorkoutsEndpoint(this WebApplication app)
        {
            app.MapGet("/api/v1/workouts", async ([FromServices] IMediator mediator, [AsParameters] GetAllWorkoutsQuery query) =>
            {
                var result = await mediator.Send(query);
                var response = new ApiResponse<PaginatedWorkoutsVm>(result, "Workouts fetched successfully");
                return Results.Ok(response);
            });
        }
    }
}
