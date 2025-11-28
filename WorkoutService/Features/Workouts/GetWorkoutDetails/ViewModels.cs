namespace WorkoutService.Features.Workouts.GetWorkoutDetails.ViewModels
{
    public class WorkoutDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Difficulty { get; set; }
        public int Duration { get; set; }
        public int CaloriesBurn { get; set; }
        public List<string> TargetMuscles { get; set; }
        public string ImageUrl { get; set; }
        public string VideoUrl { get; set; }
        public List<string> EquipmentNeeded { get; set; }
        public bool IsPremium { get; set; }
        public double? Rating { get; set; }
        public int TotalRatings { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ExerciseViewModel> Exercises { get; set; }
        public WorkoutVariationsViewModel Variations { get; set; }
        public List<string> Tips { get; set; }
    }

    public class ExerciseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int Sets { get; set; }
        public string Reps { get; set; }
        public int RestTime { get; set; }
        public string Instructions { get; set; }
    }

    public class WorkoutVariationsViewModel
    {
        public VariationViewModel Beginner { get; set; }
        public VariationViewModel Advanced { get; set; }
    }

    public class VariationViewModel
    {
        public List<string> Modifications { get; set; }
        public int EstimatedDuration { get; set; }
        public int CaloriesBurn { get; set; }
    }
}
