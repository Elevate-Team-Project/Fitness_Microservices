using Mapster;
using MediatR;
using WorkoutService.Features.Workouts.GetAllWorkouts.ViewModels;
using WorkoutService.Domain.Interfaces;
using WorkoutService.Domain.Entities;
using WorkoutService.Shared;

namespace WorkoutService.Features.Workouts.GetAllWorkouts
{
    public class GetAllWorkoutsHandler : IRequestHandler<GetAllWorkoutsQuery, PaginatedWorkoutsVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllWorkoutsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedWorkoutsVm> Handle(GetAllWorkoutsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Workouts.GetAll();

            if (!string.IsNullOrEmpty(request.Category))
            {
                query = query.Where(w => w.Category == request.Category);
            }

            if (!string.IsNullOrEmpty(request.Difficulty))
            {
                query = query.Where(w => w.Difficulty == request.Difficulty);
            }

            if (request.Duration.HasValue)
            {
                query = query.Where(w => w.Duration <= request.Duration.Value);
            }

            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(w => w.Name.Contains(request.Search) || w.Description.Contains(request.Search));
            }

            var pagedResult = PagedResult<Workout>.Create(query, request.Page, request.PageSize);

            var workoutVms = pagedResult.Items.Adapt<List<WorkoutResponseViewModel>>();

            return new PaginatedWorkoutsVm
            {
                Items = workoutVms,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages,
                HasPrevious = pagedResult.HasPrevious,
                HasNext = pagedResult.HasNext
            };
        }
    }
}
