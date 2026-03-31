using MediatR;
using NutritionService.Features.Meals.AddMeal.DTOs;
using NutritionService.Features.Shared;

namespace NutritionService.Features.Meals.AddMeal
{
    public class AddMealCommand : IRequest<EndpointResponse<int>>
    {
        public AddMealDto Meal { get; set; } = default!;
    }
}
