namespace WorkoutService.Contracts
{
    public interface IWorkoutCreated
    {
        int WorkoutId { get; }
        string Name { get; }
        string Description { get; }
        DateTime CreatedAt { get; }
    }
}
