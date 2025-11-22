namespace ProgressTrackingService.Feature.UserStatistics.CreateUserStatistics.DTOs
{
    public class UserStatisticsResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double CurrentWeight { get; set; }
        public double GoalWeight { get; set; }
    }
}