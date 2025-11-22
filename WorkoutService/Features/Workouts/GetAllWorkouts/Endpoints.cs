using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory; // ✅ Added
using WorkoutService.Features.Shared;

namespace WorkoutService.Features.Workouts.GetAllWorkouts
{
    public static class Endpoints
    {
        public static void MapGetAllWorkoutsEndpoint(this WebApplication app)
        {
            app.MapGet("/api/v1/workouts", async (
                [FromServices] IMediator mediator,
                [FromServices] IMemoryCache cache, // ✅ Injected Cache
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 20,
                [FromQuery] string? category = null,
                [FromQuery] string? difficulty = null,
                [FromQuery] int? duration = null,
                [FromQuery] string? search = null) =>
            {
                // ✅ Create a unique cache key based on all parameters
                var cacheKey = $"Workouts_Pg{page}_Sz{pageSize}_Cat{category}_Dif{difficulty}_Dur{duration}_Src{search}";

                var result = await cache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    // ✅ Set cache expiration (e.g., 5 minutes)
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                    var query = new GetAllWorkoutsQuery(page, pageSize, category, difficulty, duration, search);
                    return await mediator.Send(query);
                });

                // ✅ Check result (Note: result might come from cache or DB)
                if (result is null || !result.IsSuccess)
                {
                    return EndpointResponse<object>.ErrorResponse(
                        message: "Failed to fetch workouts",
                        errors: new List<string> { result?.Message ?? "Unknown error" }
                    );
                }

                return EndpointResponse<object>.SuccessResponse(
                    data: result.Data,
                    message: "Workouts fetched successfully"
                );
            });
        }
    }
}