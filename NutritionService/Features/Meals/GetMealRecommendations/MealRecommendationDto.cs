namespace NutritionService.Features.Meals.GetMealRecommendations
{
    public class MealRecommendationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string MealType { get; set; } = default!;
        public int Calories { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fats { get; set; }
        public string ImageUrl { get; set; } = default!;
        public int PrepTime { get; set; }
        public string Difficulty { get; set; } = default!;
        public bool IsPremium { get; set; } = false;

    }
}
