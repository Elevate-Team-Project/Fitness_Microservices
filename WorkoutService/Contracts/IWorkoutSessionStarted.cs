namespace WorkoutService.Contracts
{
    public interface IWorkoutSessionStarted
    {
        int SessionId { get; }
        int WorkoutId { get; }
        Guid UserId { get; }
        DateTime StartedAt { get; }
    }
}
