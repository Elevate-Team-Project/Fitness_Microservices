namespace WorkoutService.Contracts
{
    public interface IWorkoutSessionStarted
    {
        int WorkoutId { get; }
        Guid UserId { get; }
        int PlannedDurationMinutes { get; }
        string Difficulty { get; }
        DateTime StartedAt { get; }
    }
}
