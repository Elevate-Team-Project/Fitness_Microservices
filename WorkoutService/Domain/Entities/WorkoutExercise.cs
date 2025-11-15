namespace WorkoutService.Domain.Entities
{
    /// <summary>
    /// This is the join table for the many-to-many relationship 
    /// between Workout and Exercise. It holds the "payload" data,
    /// such as reps, sets, and order.
    /// </summary>
    public class WorkoutExercise : BaseEntity
    {
        // Composite Primary Key (configured in DbContext)
        public int WorkoutId { get; set; }
        public Workout Workout { get; set; } = null!;

        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; } = null!;

        // --- Payload Data (Properties of the relationship) ---

        /// <summary>
        /// The order of this exercise within the workout (e.g., 1, 2, 3...).
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Number of sets.
        /// </summary>
        public int Sets { get; set; }

        /// <summary>
        /// Reps description (e.g., "12-15" or "30s").
        /// </summary>
        public string Reps { get; set; } = string.Empty;

        /// <summary>
        /// Rest time in seconds after this exercise.
        /// </summary>
        public int RestTimeInSeconds { get; set; }

        /// <summary>
        /// Specific instructions for this exercise in this workout (optional).
        /// </summary>
        public string? Instructions { get; set; }
    }
}