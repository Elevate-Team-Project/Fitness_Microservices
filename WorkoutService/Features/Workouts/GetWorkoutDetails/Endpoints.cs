using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory; // ✅ Added
using WorkoutService.Features.Shared;
using WorkoutService.Features.Workouts.GetWorkoutDetails.ViewModels;

namespace WorkoutService.Features.Workouts.GetWorkoutDetails
{
    public static class Endpoints
    {
        public static void MapGetWorkoutDetailsEndpoint(this WebApplication app)
        {
            app.MapGet("/api/v1/workouts/{id}", async (
                [FromRoute] int id,
                [FromServices] IMediator mediator,
                [FromServices] IMemoryCache cache) => // ✅ Injected Cache
            {
                var cacheKey = $"WorkoutDetails_{id}"; // ✅ Unique Key

                var result = await cache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10); // 10 mins cache
                    var query = new GetWorkoutDetailsQuery(id);
                    return await mediator.Send(query);
                });

                if (result is null || !result.IsSuccess)
                {
                    var response = EndpointResponse<object>.NotFoundResponse(result?.Message ?? "Not Found");
                    return Results.Json(response, statusCode: response.StatusCode);
                }

                var success = EndpointResponse<WorkoutDetailsViewModel>.SuccessResponse(
                    result.Data!,
                    result.Message
                );

                return Results.Json(success, statusCode: success.StatusCode);
            });
        }
    }
}