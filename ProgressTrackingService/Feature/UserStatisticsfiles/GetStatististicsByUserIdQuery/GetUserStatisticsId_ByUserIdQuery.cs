using MediatR;

namespace ProgressTrackingService.Feature.UserStatisticsfiles.GetByUserIdQuery
{
    public record GetUserStatisticsId_ByUserIdQuery (int userId): IRequest<int>;


}
