using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Interfaces;
using WorkoutService.Features.Shared;
using WorkoutService.Features.Workouts.GetAllWorkouts.ViewModels;

namespace WorkoutService.Features.Workouts.GetAllWorkouts
{
    public class GetAllWorkoutsHandler : IRequestHandler<GetAllWorkoutsQuery, RequestResponse<PaginatedResult<WorkoutViewModel>>>
    {
        private readonly IBaseRepository<Workout> _workoutRepository;
        private readonly IMemoryCache _cache;

        public GetAllWorkoutsHandler(IBaseRepository<Workout> workoutRepository, IMemoryCache cache)
        {
            _workoutRepository = workoutRepository;
            _cache = cache;
        }

        public async Task<RequestResponse<PaginatedResult<WorkoutViewModel>>> Handle(GetAllWorkoutsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Workouts_Pg{request.Page}_Sz{request.PageSize}_Cat{request.Category}_Dif{request.Difficulty}_Dur{request.Duration}_Ser{request.Search}";

            var paginatedResult = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                entry.SlidingExpiration = TimeSpan.FromMinutes(2);

                var predicate = PredicateBuilder.New<Workout>(true);

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

                var query = _workoutRepository.Get(predicate).AsNoTracking();

                var pagedQuery = query.Select(w => new WorkoutViewModel
                {
                    Id = w.Id,
                    Name = w.Name,
                    Category = w.Category,
                    Difficulty = w.Difficulty,
                    Duration = w.DurationInMinutes,
                    CaloriesBurn = w.CaloriesBurn,
                    ExerciseCount = w.WorkoutExercises.Count(),
                    IsPremium = w.IsPremium,
                    Rating = w.Rating,
                    Description = w.Description,
                    TotalRatings = w.TotalRatings
                });

                var totalCount = await query.CountAsync(cancellationToken);

                var workoutVms = await pagedQuery
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                return new PaginatedResult<WorkoutViewModel>(workoutVms, totalCount, request.Page, request.PageSize);
            });

            return RequestResponse<PaginatedResult<WorkoutViewModel>>.Success(paginatedResult, "Workouts fetched successfully");
        }
    }
}