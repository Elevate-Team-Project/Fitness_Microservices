using MediatR;
using ProgressTrackingService.Domain.Entity;
using ProgressTrackingService.Domain.Interfaces;
using ProgressTrackingService.Feature.UserStatistics.CreateUserStatistics;
using ProgressTrackingService.Feature.UserStatistics.CreateUserStatistics.DTOs;

namespace ProgressTrackingService.Feature.UserStatisticsfiles.CreateUserStatistics
{
    public class CreateUserStatisticsCommandHandler : IRequestHandler<CreateUserStatisticsCommand, UserStatistics.CreateUserStatistics.DTOs.UserStatisticsResponseDto>
    {
        private readonly IGenericRepository<Domain.Entity.UserStatistics> _repository;
        private readonly IUniteOfWork _uof;

        public CreateUserStatisticsCommandHandler(IGenericRepository<Domain.Entity.UserStatistics> repository,IUniteOfWork uniteOfWork )
        {
            _repository = repository;
            _uof = uniteOfWork;
        }
       

        async Task<UserStatisticsResponseDto> IRequestHandler<CreateUserStatisticsCommand, UserStatisticsResponseDto>.Handle(CreateUserStatisticsCommand request, CancellationToken cancellationToken)
        {
            var userStatistics = new Domain.Entity.UserStatistics 
            {

                UserId = request.userId,
                StartingWeight = request.currentWeight,
                GoalWeight = request.goalWeight,
                LatestWeight = 0,
                TotalWorkouts = 0,
                TotalCaloriesBurned = 0,
                CurrentStreak = 0,
                LongestStreak = 0,
                UpdatedAt = DateTime.UtcNow
            };

            var UserStatistics = await _repository.AddAsync(userStatistics);
            await _uof.SaveChangesAsync();
            return new UserStatisticsResponseDto
            {
                Id = UserStatistics.Id,
                UserId = UserStatistics.UserId,
                CurrentWeight = UserStatistics.StartingWeight,
                GoalWeight = UserStatistics.GoalWeight
            };
        }
    }
}
