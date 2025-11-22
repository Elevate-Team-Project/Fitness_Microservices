namespace ProgressTrackingService.Domain.Entity
{
    public class UserAchievement : BaseEntity
    {
        
        public int UserId { get; set; }
        public int AchievementId { get; set; }
        public DateTime EarnedAt { get; set; }
        public Achievement Achievement { get; set; }



    }
}
