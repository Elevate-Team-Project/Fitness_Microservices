namespace ProgressTrackingService.Domain.Entity
{
    public class Achievement : BaseEntity
    {
       
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; }
        public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();


    }
}
