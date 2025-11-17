namespace NutritionService.Domain.Models
{
    public class MealPlans : BaseEntity 
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int CalorieTarget { get; set; }

    }
}
