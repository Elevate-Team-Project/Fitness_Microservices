namespace WorkoutService.Features.Workouts.CreateWorkout
{
    public record CreateWorkoutDto(string Name, string Description , int CaloriesBurn , bool IsPremium , double? Rating ,
        int DurationInMinutes, string Difficulty ,string Category
        , int workoutPlanId
        );
}
