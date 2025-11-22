namespace ProgressTrackingService.Domain.Entity
{
    public class WorkoutLog : BaseEntity 
    {
        
        public int UserId { get; set; }
        public int WorkoutId{ get; set; } 
        public string WorkoutName { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty; // e.g., "Cardio", "Strength"
        public int CaloriesBurned { get; set; }
        public int Duration { get; set; } // in minutes
        public int Rating { get; set; } // 1 to 5
        public string Notes { get; set; } = string.Empty;
        public DateTime CompletedAt { get; set; } = DateTime.Now;

    }
}
