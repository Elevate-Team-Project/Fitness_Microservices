using ProgressTrackingService.Feature.LogWorkout.CreateWorkoutLogCommand.DTOs;

namespace ProgressTrackingService.Feature.LogWorkout.PlaceWorkoutOrchestrator.DTos
{
    public class WorkoutLogResponseDto
    {
        public int Id { get; set; }
        public int WorkoutId { get; set; }
        public string WorkoutName { get; set; }
        public DateTime CompletedAt { get; set; }
        public int Duration { get; set; } // minutes
        public int CaloriesBurned { get; set; }
        public int CurrentStreak { get; set; }//statistics
        public int TotalWorkouts { get; set; }//statistics
        public int TotalCaloriesBurned { get; set; }//statistics
        public List<AchievementDto> NewAchievements { get; set; }

    }
}
