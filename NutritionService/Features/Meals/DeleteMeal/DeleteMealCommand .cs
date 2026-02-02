using MediatR;
using NutritionService.Features.Shared;

namespace NutritionService.Features.Meals.DeleteMeal
{
    public class DeleteMealCommand : IRequest<EndpointResponse<bool>>
    {
        public int Id { get; set; }
    }
}
