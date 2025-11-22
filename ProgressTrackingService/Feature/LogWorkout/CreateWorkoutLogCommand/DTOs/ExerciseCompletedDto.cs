namespace ProgressTrackingService.Feature.LogWorkout.CreateWorkoutLogCommand.DTOs
{
    public class ExerciseCompletedDto
    {
        public int ExerciseId { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public bool Completed { get; set; }

    }
}