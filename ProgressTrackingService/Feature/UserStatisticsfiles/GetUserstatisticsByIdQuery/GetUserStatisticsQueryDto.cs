namespace ProgressTrackingService.Feature.UserStatisticsfiles.GetUserstatisticsByIdQuery
{
    public class GetUserStatisticsQueryDto
    {
        
        public int TotalWorkouts { get; set; }
        public int TotalCaloriesBurned { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public double LatestWeight { get; set; }
        public double StartingWeight { get; set; }
        public double GoalWeight { get; set; }
    }
}
