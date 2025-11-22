using MediatR;
using ProgressTrackingService.Feature.UserStatistics.CreateUserStatistics.DTOs;

namespace ProgressTrackingService.Feature.UserStatistics.CreateUserStatistics
{
    public record CreateUserStatisticsCommand(int userId,double currentWeight,double goalWeight): IRequest<UserStatisticsResponseDto>;

}
