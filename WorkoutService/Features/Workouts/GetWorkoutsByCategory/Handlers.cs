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
            // 1. AsNoTracking is recommended for read-only queries to avoid change tracking overhead
            var query = _workoutRepository.GetAll()
                .AsNoTracking()
                .Where(w => w.Category == request.CategoryName);

            if (!string.IsNullOrEmpty(request.Difficulty))
            {
                query = query.Where(w => w.Difficulty == request.Difficulty);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            // 2. Core Optimization: Use Manual Projection (.Select)
            // This ensures only the required columns are fetched from the database (SQL SELECT)
            // instead of fetching the entire Entity.
            var workoutVms = await query
               .Skip((request.Page - 1) * request.PageSize)
               .Take(request.PageSize)
               .Select(w => new WorkoutViewModel
               {
                   Id = w.Id,
                   Name = w.Name,
                   Description = w.Description, // Fetch only required properties
                   Difficulty = w.Difficulty,
                   Rating = w.Rating
                   // Map the remaining properties manually here...
               })
               .ToListAsync(cancellationToken);

            var paginatedResult = new PaginatedResult<WorkoutViewModel>(workoutVms, totalCount, request.Page, request.PageSize);

            return RequestResponse<PaginatedResult<WorkoutViewModel>>.Success(paginatedResult, "Category workouts fetched successfully");
        }
    }
}