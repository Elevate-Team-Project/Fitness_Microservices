namespace WorkoutService.Domain.Entities
{
    /// <summary>
    /// Represents a single exercise (e.g., "Push-up", "Squat").
    /// </summary>
    public class Exercise : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }

        // Store collections as JSON strings in the DB
        public List<string> TargetMuscles { get; set; } = new List<string>();
        public List<string> EquipmentNeeded { get; set; } = new List<string>();

        // --- Relationships ---

        // Navigation property for the join table (many-to-many with Workout)
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
    }
}