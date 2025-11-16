using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkoutService.Features.Shared;
using WorkoutService.Features.Workouts.GetWorkoutDetails.ViewModels;

namespace WorkoutService.Features.Workouts.GetWorkoutDetails
{
    public static class Endpoints
    {
        public static void MapGetWorkoutDetailsEndpoint(this WebApplication app)
        {
            app.MapGet("/api/v1/workouts/{id}", async (
                [FromServices] IMediator mediator,
                [FromRoute] int id) =>
            {
                var query = new GetWorkoutDetailsQuery(id);
                var result = await mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return Results.NotFound(EndpointResponse<object>.NotFoundResponse(result.Message));
                }

                return Results.Ok(EndpointResponse<WorkoutDetailsViewModel>.SuccessResponse(
                    data: result.Data,
                    message: "Workout details fetched successfully"
                ));
            });
        }
    }
}
