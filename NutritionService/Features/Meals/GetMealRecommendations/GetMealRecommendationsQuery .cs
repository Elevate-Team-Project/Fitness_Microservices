using MediatR;
using NutritionService.Features.Shared;

namespace NutritionService.Features.Meals.GetMealRecommendations
{
    public class GetMealRecommendationsQuery : IRequest<PaginatedResult<MealRecommendationDto>>
    {
        public string? MealType { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int? MaxCalories { get; set; }
        public int? MinProtein { get; set; }
    }


}
