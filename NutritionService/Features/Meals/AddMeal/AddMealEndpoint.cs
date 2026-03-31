using MediatR;
using NutritionService.Features.Meals.AddMeal.DTOs;

namespace NutritionService.Features.Meals.AddMeal
{
    public static class AddMealEndpoint
    {
        public static void MapAddMealEndpoint(this WebApplication app)
        {
            app.MapPost("/api/v1/nutrition/meals",
                async (AddMealDto dto, IMediator mediator) =>
                {
                    var cmd = new AddMealCommand { Meal = dto };
                    var result = await mediator.Send(cmd);
                    return Results.Ok(result);
                });
        }
    }
}
