using MediatR;
using ProgressTrackingService.Domain.Interfaces;

namespace ProgressTrackingService.Feature.UserStatisticsfiles.UpdateUserStatistics
{
    public class updateUserStatisticsCommandHandler
        : IRequestHandler<UpdateUserstatisticsCommand, bool>
    {
        private readonly IGenericRepository<Domain.Entity.UserStatistics> _repository;
        private readonly IUniteOfWork _unitOfWork;

        public updateUserStatisticsCommandHandler(
            IGenericRepository<Domain.Entity.UserStatistics> repository,
            IUniteOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateUserstatisticsCommand request, CancellationToken cancellationToken)
        {
            var existing = _repository.GetAll()
                .FirstOrDefault(us => us.UserId == request.userId);

            if (existing is null)
                return false;

            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);
            var lastUpdate = existing.UpdatedAt?.Date; // SAFE nullable handling

            var updatedStats = new Domain.Entity.UserStatistics
            {
                Id = existing.Id,
                TotalWorkouts = existing.TotalWorkouts + 1,
                TotalCaloriesBurned = existing.TotalCaloriesBurned + request.CaloriesBurned,
                UpdatedAt = today
            };

            var includeFields = new List<string>
            {
                "TotalWorkouts",
                "TotalCaloriesBurned",
                "UpdatedAt"
            };

            // ========== Streak Logic ==========
            if (lastUpdate == null)
            {
                // First time ever
                updatedStats.CurrentStreak = 1;
                updatedStats.LongestStreak = 1;
                includeFields.Add("CurrentStreak");
                includeFields.Add("LongestStreak");
            }
            else if (lastUpdate == yesterday)
            {
                // Continue streak
                updatedStats.CurrentStreak = existing.CurrentStreak + 1;
                includeFields.Add("CurrentStreak");

                if (updatedStats.CurrentStreak > existing.LongestStreak)
                {
                    updatedStats.LongestStreak = updatedStats.CurrentStreak;
                    includeFields.Add("LongestStreak");
                }
            }
            else if (lastUpdate < yesterday)
            {
                // Reset streak - but first check if previous streak was longer than longest
                if (existing.CurrentStreak > existing.LongestStreak)
                {
                    updatedStats.LongestStreak = existing.CurrentStreak;
                    includeFields.Add("LongestStreak");
                }
                updatedStats.CurrentStreak = 1;
                includeFields.Add("CurrentStreak");
            }
           

           
            _repository.SaveInclude(updatedStats, includeFields.ToArray());

            var saved = await _unitOfWork.SaveChangesAsync();
            return saved > 0;
        }
    }
}
