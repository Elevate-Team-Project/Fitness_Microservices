using MediatR;
using NutritionService.Domain.Interfaces;
using NutritionService.Domain.Models;
using NutritionService.Features.Shared;

namespace NutritionService.Features.Meals.DeleteMeal
{
    public class DeleteMealHandler : IRequestHandler<DeleteMealCommand, EndpointResponse<bool>>
    {
        private readonly IUnitOfWork _uow;

        public DeleteMealHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EndpointResponse<bool>> Handle(DeleteMealCommand request, CancellationToken cancellationToken)
        {
            var meal = await _uow.GetRepository<Meal>().GetByIdAsync(request.Id);

            if (meal is null)
                return EndpointResponse<bool>.NotFoundResponse($"Meal with id {request.Id} not found");

            _uow.GetRepository<Meal>().Delete(meal);
            await _uow.SaveChangesAsync();

            return EndpointResponse<bool>.SuccessResponse(true, "Meal deleted successfully");
        }
    }
}
