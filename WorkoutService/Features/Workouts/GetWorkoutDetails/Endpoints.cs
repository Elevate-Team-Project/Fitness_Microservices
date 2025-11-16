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
                    return Results.NotFound(new EndpointResponse<object>(
                        false,
                        result.Message,
                        null,
                        new List<string> { result.Message },
                        404,
                        DateTime.UtcNow
                    ));
                }

                return Results.Ok(new EndpointResponse<WorkoutDetailsViewModel>(
                    true,
                    result.Message,
                    result.Data,
                    null,
                    200,
                    DateTime.UtcNow
                ));
            });
        }
    }
}
