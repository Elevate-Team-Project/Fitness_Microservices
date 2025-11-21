using MediatR;
using NutritionService.Features.Meals.Filters;
using NutritionService.Features.Shared;
using NutritionService.Infrastructure.Data;

namespace NutritionService.Features.Meals.GetMealRecommendations
{
    public class GetMealRecommendationsHandlers
        : IRequestHandler<GetMealRecommendationsQuery, PaginatedResult<MealRecommendationDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetMealRecommendationsHandlers(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<MealRecommendationDto>> Handle(
            GetMealRecommendationsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.meals
                .Where(m => !m.IsDeleted)
                .AsQueryable();

            query = MealFilter.ApplyFilters(
                query,
                request.MealType,
                request.MaxCalories,
                request.MinProtein
            );

            var projectedQuery = query.Select(m => new MealRecommendationDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                MealType = m.mealType.ToString(),

                Calories = m.NutritionFacts.Calories,
                Protein = m.NutritionFacts.Protein,
                Carbs = m.NutritionFacts.Carbs,
                Fats = m.NutritionFacts.Fats,

                PrepTime = m.PrepTimeInMinutes,
                ImageUrl = m.ImageUrl,
                Difficulty = m.Difficulty,
                IsPremium = m.IsPremium
            });

            return await projectedQuery.ToPaginatedResultAsync(
               request.Page,
               request.PageSize,
               cancellationToken
           );
        }
    }
}