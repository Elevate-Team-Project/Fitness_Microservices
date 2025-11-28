using System;
using WorkoutService.Domain.Entities; // Ensure BaseEntity is accessible

namespace WorkoutService.Domain.Entities
{
    public class WorkoutSessionExercise : BaseEntity
    {
        public int WorkoutSessionId { get; set; } // Note: Check if your Session ID is int or Guid based on your Session class
        public WorkoutSession WorkoutSession { get; set; } = null!;

        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; } = null!;

        public string Status { get; set; } = "Pending"; // e.g., "Pending", "Completed"
        public int Order { get; set; }

        // ✅✅✅ Add these properties to fix the error ✅✅✅
        // These are needed to store the "Target" for this specific session
        public int Sets { get; set; }
        public string Reps { get; set; } = string.Empty;
        public int RestTimeInSeconds { get; set; }
    }
}