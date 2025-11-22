using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory; // ✅ Added

namespace WorkoutService.Features.WorkoutPlans.GetAllWorkoutPlans
{
    public static class Endpoints
    {
        public static void MapGetAllWorkoutPlansEndpoint(this WebApplication app)
        {
            app.MapGet("/workout-plans", async (
                [FromServices] IMediator mediator,
                [FromServices] IMemoryCache cache) => // ✅ Injected Cache
            {
                var cacheKey = "GetAllWorkoutPlans";

                var result = await cache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15); // Long cache
                    var query = new GetAllWorkoutPlansQuery();
                    return await mediator.Send(query);
                });

                return Results.Ok(result);
            });
        }
    }
}