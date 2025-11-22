using MediatR;

namespace ProgressTrackingService.Feature.UserStatisticsfiles.GetUserstatisticsByIdQuery
{
    public record GetUserStatisticsQuery(int UserStatisticId) : IRequest<GetUserStatisticsQueryDto>;

}
