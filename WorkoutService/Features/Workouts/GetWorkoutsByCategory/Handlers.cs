using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Interfaces;
using WorkoutService.Features.Shared;
using WorkoutService.Features.Workouts.GetAllWorkouts.ViewModels;

namespace WorkoutService.Features.Workouts.GetWorkoutsByCategory
{
    public class GetWorkoutsByCategoryHandler : IRequestHandler<GetWorkoutsByCategoryQuery, RequestResponse<PaginatedResult<WorkoutViewModel>>>
    {
        private readonly IBaseRepository<Workout> _workoutRepository;

        public GetWorkoutsByCategoryHandler(IBaseRepository<Workout> workoutRepository)
        {
            _workoutRepository = workoutRepository;
        }

        public async Task<RequestResponse<PaginatedResult<WorkoutViewModel>>> Handle(GetWorkoutsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var query = _workoutRepository.GetAll().Where(w => w.Category == request.CategoryName);

            if (!string.IsNullOrEmpty(request.Difficulty))
            {
                query = query.Where(w => w.Difficulty == request.Difficulty);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var workouts = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var workoutVms = workouts.Adapt<List<WorkoutViewModel>>();
            var paginatedResult = new PaginatedResult<WorkoutViewModel>(workoutVms, totalCount, request.Page, request.PageSize);

            return RequestResponse<PaginatedResult<WorkoutViewModel>>.Success(paginatedResult, "Category workouts fetched successfully");
        }
    }
}
