namespace WorkoutService.Features.Workouts.StartWorkoutSession.ViewModels
{
    public class WorkoutSessionViewModel
    {
        public string SessionId { get; set; }
        public int WorkoutId { get; set; }
        public string WorkoutName { get; set; }
        public DateTime StartedAt { get; set; }
        public int PlannedDuration { get; set; }
        public string Difficulty { get; set; }
        public List<SessionExerciseViewModel> Exercises { get; set; }
    }

    public class SessionExerciseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int Sets { get; set; }
        public string Reps { get; set; }
        public int RestTime { get; set; }
        public bool Completed { get; set; }
    }
}
