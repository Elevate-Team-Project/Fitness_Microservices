using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Interfaces;
using WorkoutService.Features.Shared;
using WorkoutService.Features.Workouts.GetWorkoutDetails.ViewModels;

namespace WorkoutService.Features.Workouts.GetWorkoutDetails
{
    public class GetWorkoutDetailsHandler : IRequestHandler<GetWorkoutDetailsQuery, RequestResponse<WorkoutDetailsViewModel>>
    {
        private readonly IBaseRepository<Workout> _workoutRepository;


        private static readonly WorkoutVariationsViewModel _defaultVariations = new()
        {
            Beginner = new VariationViewModel
            {
                Modifications = new List<string> { "Knee push-ups" },
                EstimatedDuration = 35,
                CaloriesBurn = 250
            },
            Advanced = new VariationViewModel
            {
                Modifications = new List<string> { "Weighted push-ups" },
                EstimatedDuration = 55,
                CaloriesBurn = 450
            }
        };

        private static readonly List<string> _defaultTips = new()
        {
            "Warm up properly",
            "Focus on form"
        };

        public GetWorkoutDetailsHandler(IBaseRepository<Workout> workoutRepository)
        {
            _workoutRepository = workoutRepository;
        }

        public async Task<RequestResponse<WorkoutDetailsViewModel>> Handle(GetWorkoutDetailsQuery request, CancellationToken cancellationToken)
        {
            var workoutDetailsVm = await _workoutRepository.GetAll()
                .AsNoTracking()
                .Where(w => w.Id == request.Id)
                .Select(w => new WorkoutDetailsViewModel
                {
                    Id = w.Id,
                    Name = w.Name,
                    Description = w.Description,
                    Category = w.Category,
                    Difficulty = w.Difficulty,
                    Duration = w.DurationInMinutes,
                    CaloriesBurn = w.CaloriesBurn,
                    IsPremium = w.IsPremium,
                    Rating = w.Rating,
                    Exercises = w.WorkoutExercises
                        .OrderBy(we => we.Order) 
                        .Select(we => new ExerciseViewModel
                        {
                            Id = we.ExerciseId,
                            Name = we.Exercise.Name,
                            Sets = we.Sets,
                            Reps = we.Reps,
                            RestTime = we.RestTimeInSeconds,
                            Order = we.Order
                        }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (workoutDetailsVm == null)
            {
                return RequestResponse<WorkoutDetailsViewModel>.Fail("Workout not found");
            }

            workoutDetailsVm.Variations = _defaultVariations;
            workoutDetailsVm.Tips = _defaultTips;

            return RequestResponse<WorkoutDetailsViewModel>.Success(workoutDetailsVm, "Workout details fetched successfully");
        }
    }
}