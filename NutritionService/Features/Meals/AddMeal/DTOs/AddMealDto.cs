using NutritionService.Features.Meals.GetMealDetails.DTOs;

namespace NutritionService.Features.Meals.AddMeal.DTOs
{
    public class AddMealDto
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string MealType { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public int PrepTime { get; set; }
        public string Difficulty { get; set; } = default!;
        public bool IsPremium { get; set; }

        public NutritionDto Nutrition { get; set; } = default!;
        public List<IngredientDto> Ingredients { get; set; } = new();
    }

}
