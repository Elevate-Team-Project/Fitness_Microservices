using LinqKit; // ✅ Required for PredicateBuilder
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Interfaces;
using WorkoutService.Features.Shared;
using WorkoutService.Features.Workouts.GetAllWorkouts.ViewModels;
using System.Linq.Expressions;

namespace WorkoutService.Features.Workouts.GetAllWorkouts
{
    public class GetAllWorkoutsHandler : IRequestHandler<GetAllWorkoutsQuery, RequestResponse<PaginatedResult<WorkoutViewModel>>>
    {
        private readonly IBaseRepository<Workout> _workoutRepository;

        public GetAllWorkoutsHandler(IBaseRepository<Workout> workoutRepository)
        {
            _workoutRepository = workoutRepository;
        }

        public async Task<RequestResponse<PaginatedResult<WorkoutViewModel>>> Handle(GetAllWorkoutsQuery request, CancellationToken cancellationToken)
        {
            // 1. Initialize the Predicate Builder (Start with True for AND logic)
            var predicate = PredicateBuilder.New<Workout>(true);

            // 2. Build the Expression Tree dynamically
            if (!string.IsNullOrEmpty(request.Category))
            {
                predicate.And(w => w.Category == request.Category);
            }

            if (!string.IsNullOrEmpty(request.Difficulty))
            {
                predicate.And(w => w.Difficulty == request.Difficulty);
            }

            if (request.Duration.HasValue)
            {
                predicate.And(w => w.DurationInMinutes == request.Duration.Value);
            }

            if (!string.IsNullOrEmpty(request.Search))
            {
                predicate.And(w => w.Name.Contains(request.Search) || w.Description.Contains(request.Search));
            }

            // 3. Pass the 'Tree' to the Repository & Optimization
            var query = _workoutRepository.Get(predicate)
                .AsNoTracking(); // ✅ Important: No tracking overhead

            // 4. Projection 
            var pagedQuery = query
                .Select(w => new WorkoutViewModel
                {
                    Id = w.Id,
                    Name = w.Name,
                    Category = w.Category,
                    Difficulty = w.Difficulty,
                    Duration = w.DurationInMinutes,
                    CaloriesBurn = w.CaloriesBurn,
                    ExerciseCount = w.WorkoutExercises.Count(), // ✅ Efficient SQL Count
                    IsPremium = w.IsPremium,
                    Rating = w.Rating,
                    Description = w.Description,
                    TotalRatings = w.TotalRatings
                });

            var totalCount = await query.CountAsync(cancellationToken);

            // This will now create a List of Structs (One contiguous block of memory)
            var workoutVms = await pagedQuery
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var paginatedResult = new PaginatedResult<WorkoutViewModel>(workoutVms, totalCount, request.Page, request.PageSize);

            return RequestResponse<PaginatedResult<WorkoutViewModel>>.Success(paginatedResult, "Workouts fetched successfully");
        }
    }
}