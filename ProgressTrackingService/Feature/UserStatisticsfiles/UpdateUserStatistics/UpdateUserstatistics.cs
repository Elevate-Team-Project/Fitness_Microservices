using MediatR;

namespace ProgressTrackingService.Feature.UserStatisticsfiles.UpdateUserStatistics
{
    public record UpdateUserstatisticsCommand(int CaloriesBurned,int userId): IRequest<bool>;


}
