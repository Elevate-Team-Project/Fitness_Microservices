using MediatR;

namespace ProgressTrackingService.Feature.Achievement.CreateAchievement
{
    public record CreateAchievementCommand(int UserId, string Title, string Description, DateTime DateAchieved) : IRequest<int>;


}
