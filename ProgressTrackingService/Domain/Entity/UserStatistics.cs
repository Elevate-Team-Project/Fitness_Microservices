using System.ComponentModel.DataAnnotations;

namespace ProgressTrackingService.Domain.Entity
{
    public class UserStatistics : BaseEntity
    {
        
         
        public int UserId { get; set; }
        public int TotalWorkouts { get; set; }
        public int TotalCaloriesBurned { get; set; }   
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public double LatestWeight { get; set; }
        public double StartingWeight { get; set; }
        public double GoalWeight { get; set; }
       


    }
}
