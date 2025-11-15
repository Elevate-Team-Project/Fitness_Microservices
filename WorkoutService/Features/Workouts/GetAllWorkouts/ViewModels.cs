using System.Collections.Generic;
using WorkoutService.Shared;

namespace WorkoutService.Features.Workouts.GetAllWorkouts.ViewModels
{
    public class PaginatedWorkoutsVm
    {
        public List<WorkoutResponseViewModel> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }

    public class WorkoutResponseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Difficulty { get; set; }
        public int Duration { get; set; }
        public int CaloriesBurn { get; set; }
        public int ExerciseCount { get; set; }
        public List<string> TargetMuscles { get; set; }
        public string ImageUrl { get; set; }
        public string VideoUrl { get; set; }
        public List<string> EquipmentNeeded { get; set; }
        public bool IsPremium { get; set; }
        public double Rating { get; set; }
        public int TotalRatings { get; set; }
    }
}
