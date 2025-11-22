using MediatR;
using ProgressTrackingService.Domain.Entity;
using ProgressTrackingService.Domain.Interfaces;

namespace ProgressTrackingService.Feature.UserStatisticsfiles.GetUserstatisticsByIdQuery
{
    public class Handler : IRequestHandler<GetUserStatisticsQuery, GetUserStatisticsQueryDto>
    {
        private readonly IGenericRepository<Domain.Entity.UserStatistics> _repository;

        public Handler(IGenericRepository<Domain.Entity.UserStatistics>repository)
        {
            this._repository = repository;
        }
        public Task<GetUserStatisticsQueryDto> Handle(GetUserStatisticsQuery request, CancellationToken cancellationToken)
        {
            var userStatistics = _repository.GetByIdAsync(request.UserStatisticId).Result;
            if (userStatistics == null)
            {
                return Task.FromResult<GetUserStatisticsQueryDto>(null);
            }

            var dto = new GetUserStatisticsQueryDto
            {
                TotalWorkouts = userStatistics.TotalWorkouts,
                TotalCaloriesBurned = userStatistics.TotalCaloriesBurned,
                CurrentStreak = userStatistics.CurrentStreak,
                LongestStreak = userStatistics.LongestStreak,
                LatestWeight = userStatistics.LatestWeight,
                StartingWeight = userStatistics.StartingWeight,
                GoalWeight = userStatistics.GoalWeight
            };
            return Task.FromResult(dto);
        }
    }
}
