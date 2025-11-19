using MediatR;
using NutritionService.Features.Shared;

namespace NutritionService.Features.Meals.GetMealRecommendations
{
    public static class GetMealRecommendationsEndpoint
    {
        public static void MapGetMealRecommendationsEndpoint(this WebApplication app)
        {
            app.MapGet("/api/v1/nutrition/recommendations",
             async ([AsParameters] GetMealRecommendationsQuery query, IMediator mediator) =>
             {
                 var result = await mediator.Send(query);
                 return Results.Ok(result);
             });


        }
    }
}
