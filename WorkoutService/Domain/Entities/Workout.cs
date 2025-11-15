namespace WorkoutService.Domain.Entities
{
    /// <summary>
    /// Represents a single workout routine (e.g., "Full Body Strength").
    /// </summary>
    public class Workout : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // e.g., "full-body", "chest"
        public string Difficulty { get; set; } = string.Empty; // e.g., "Beginner", "Intermediate"
        public int DurationInMinutes { get; set; }
        public int CaloriesBurn { get; set; }
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public bool IsPremium { get; set; }
        public double Rating { get; set; }
        public int TotalRatings { get; set; }

        // --- Relationships ---

        // Foreign Key to WorkoutPlan
        public int WorkoutPlanId { get; set; }
        public WorkoutPlan WorkoutPlan { get; set; } = null!;

        // Navigation property for the join table (many-to-many with Exercise)
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
    }
}