using NutritionService.Domain.Models;

namespace NutritionService.Features.Meals.Filters
{
    public static class MealFilter
    {
        public static IQueryable<Meal> ApplyFilters(
            IQueryable<Meal> query,
            string? mealType,
            int? maxCalories,
            int? minProtein)
        {
            if (!string.IsNullOrWhiteSpace(mealType))
                query = query.Where(m => m.mealType.ToString() == mealType);

            if (maxCalories.HasValue)
                query = query.Where(m => m.NutritionFacts.Calories <= maxCalories);

            if (minProtein.HasValue)
                query = query.Where(m => m.NutritionFacts.Protein >= minProtein);

            return query;
        }
    }
}
