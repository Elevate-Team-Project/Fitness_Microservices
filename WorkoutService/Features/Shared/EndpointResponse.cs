namespace WorkoutService.Features.Shared
{
    public record EndpointResponse<T>(
        bool isSuccess,
        string message,
        T data,
        List<string> errors,
        int statusCode,
        DateTime timestamp
    );
}
