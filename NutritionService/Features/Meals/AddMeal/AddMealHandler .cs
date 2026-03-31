using MediatR;
using NutritionService.Domain.Interfaces;
using NutritionService.Domain.Models;
using NutritionService.Domain.Models.Enums;
using NutritionService.Features.Shared;

namespace NutritionService.Features.Meals.AddMeal
{
    public class AddMealHandler
         : IRequestHandler<AddMealCommand, EndpointResponse<int>>
    {
        private readonly IUnitOfWork _uow;

        public AddMealHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EndpointResponse<int>> Handle(AddMealCommand request, CancellationToken cancellationToken)
        {
            var data = request.Meal;

            var mealRepo = _uow.GetRepository<Meal>();
            var ingredientRepo = _uow.GetRepository<MealIngredient>();
            var nutritionRepo = _uow.GetRepository<NutritionFact>();

            var meal = new Meal
            {
                Name = data.Name,
                Description = data.Description,
                ImageUrl = data.ImageUrl,
                Difficulty = data.Difficulty,
                PrepTimeInMinutes = data.PrepTime,
                IsPremium = data.IsPremium,
                mealType = Enum.Parse<MealType>(data.MealType)
            };

            mealRepo.Create(meal);
            await _uow.SaveChangesAsync();

            var nutrition = new NutritionFact
            {
                MealId = meal.Id,
                Calories = data.Nutrition.Calories,
                Protein = data.Nutrition.Protein,
                Carbs = data.Nutrition.Carbs,
                Fats = data.Nutrition.Fats,
                Fiber = data.Nutrition.Fiber,
            };

            nutritionRepo.Create(nutrition);

            foreach (var ing in data.Ingredients)
            {
                ingredientRepo.Create(new MealIngredient
                {
                    MealId = meal.Id,
                    IngredientId = ing.IngredientId,
                    Amount = ing.Amount
                });
            }

            await _uow.SaveChangesAsync();

            return EndpointResponse<int>.SuccessResponse(meal.Id, "Meal added successfully");
        }
    }
}
