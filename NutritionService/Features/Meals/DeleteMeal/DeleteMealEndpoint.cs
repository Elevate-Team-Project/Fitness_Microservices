using MediatR;

namespace NutritionService.Features.Meals.DeleteMeal
{
    public static class DeleteMealEndpoint
    {
        public static void MapDeleteMealEndpoint(this WebApplication app)
        {
            app.MapDelete("/api/v1/nutrition/meals/{id:int}",
                async (int id, IMediator mediator) =>
                {
                    var cmd = new DeleteMealCommand { Id = id };
                    var result = await mediator.Send(cmd);
                    return Results.Ok(result);
                });
        }
    }
}
