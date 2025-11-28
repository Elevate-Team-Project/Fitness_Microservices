using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory; // ✅ Added
using WorkoutService.Features.Shared;
using WorkoutService.Features.Workouts.GetAllWorkouts.ViewModels;

namespace WorkoutService.Features.Workouts.GetWorkoutsByCategory
{
    public static class Endpoints
    {
        public static void MapGetWorkoutsByCategoryEndpoint(this WebApplication app)
        {
            app.MapGet("/api/v1/workouts/category/{categoryName}", async (
                [FromServices] IMediator mediator,
                [FromServices] IMemoryCache cache, // ✅ Injected Cache
                [FromRoute] string categoryName,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 20,
                [FromQuery] string? difficulty = null) =>
            {
                // ✅ Unique Key composition
                var cacheKey = $"WorkoutsByCat_{categoryName}_Pg{page}_Sz{pageSize}_Dif{difficulty}";

                var result = await cache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    var query = new GetWorkoutsByCategoryQuery(categoryName, page, pageSize, difficulty);
                    return await mediator.Send(query);
                });

                if (result is null || !result.IsSuccess)
                {
                    return Results.BadRequest(
                        new EndpointResponse<object>(
                            null,
                            result?.Message ?? "Error",
                            false,
                            400,
                            new List<string> { result?.Message ?? "Error" },
                            DateTime.UtcNow
                        )
                    );
                }

                return Results.Ok(
                    new EndpointResponse<PaginatedResult<WorkoutViewModel>>(
                        result.Data,
                        result.Message,
                        true,
                        200,
                        null,
                        DateTime.UtcNow
                    )
                );
            });
        }
    }
}