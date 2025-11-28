namespace WorkoutService.Contracts
{
    public interface IWorkoutCreated
    {
        int WorkoutId { get; }
        string Name { get; }
        string Description { get; }
        string Category { get; }
        string Difficulty { get; }
        int CaloriesBurn { get; }
        int DurationInMinutes { get; }
        bool IsPremium { get; }
        double Rating { get; }
        DateTime CreatedAt { get; }
        int WorkoutPlanId { get; } // ✅ Important for linking
    }
}
