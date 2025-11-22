using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory; // ✅ Added

namespace WorkoutService.Features.WorkoutPlans.GetWorkoutPlanDetails
{
    public static class Endpoints
    {
        public static void MapGetWorkoutPlanDetailsEndpoint(this WebApplication app)
        {
            app.MapGet("/workout-plans/{id}", async (
                int id,
                [FromServices] IMediator mediator,
                [FromServices] IMemoryCache cache) => // ✅ Injected Cache
            {
                var cacheKey = $"WorkoutPlanDetails_{id}";

                var result = await cache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    var query = new GetWorkoutPlanDetailsQuery(id);
                    return await mediator.Send(query);
                });

                return result is not null ? Results.Ok(result) : Results.NotFound();
            });
        }
    }
}