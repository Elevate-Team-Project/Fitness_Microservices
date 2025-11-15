namespace WorkoutService.Domain.Entities
{
    /// <summary>
    /// Represents a high-level training plan (e.g., "Weight Loss - Normal Intensity").
    /// This entity links to plans defined in the Fitness Calculation Engine.
    /// </summary>
    public class WorkoutPlan : BaseEntity
    {

        // The string-based ID from the Fitness Calculation Engine (e.g., "plan_lw_normal")
        public string ExternalPlanId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Goal { get; set; } = string.Empty; // e.g., "Lose Weight"
        public string Status { get; set; } = string.Empty; // e.g., "Normal"
        public string Difficulty { get; set; } = string.Empty;

        // --- Relationships ---

        // A WorkoutPlan can contain many workouts
        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
    }
}