using ProgressTrackingService.Feature.LogWorkout.CreateWorkoutLogCommand.DTOs;

namespace ProgressTrackingService.Feature.LogWorkout.PlaceWorkoutOrchestrator.DTos
{
    public class WorkoutLogDto
    {
        public int WorkoutId { get; set; }
        public string SessionId { get; set; }
        public int UserId { get; set; }//.
        public string WorkoutName { get; set; }
        public DateTime CompletedAt { get; set; }
        public int Duration { get; set; } // in minutes
        public int CaloriesBurned { get; set; }
        public string Difficulty { get; set; }
        public string Notes { get; set; }
        public int Rating { get; set; } // e.g., 1–5 scale
        public List<ExerciseCompletedDto> ExercisesCompleted { get; set; }

    }
}
