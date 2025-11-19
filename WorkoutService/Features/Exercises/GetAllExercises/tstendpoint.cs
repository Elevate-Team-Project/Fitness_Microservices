namespace WorkoutService.Features.Exercises.GetAllExercises
{
    public static class tstendpoint
    {
        public static void MaptstEndpoint(this WebApplication app)
        {
            app.MapGet("/tst", () => "tst");
        }
    }
}
