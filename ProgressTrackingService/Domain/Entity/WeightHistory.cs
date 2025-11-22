namespace ProgressTrackingService.Domain.Entity
{
    public class WeightHistory : BaseEntity
    {
        
        public int UserId { get; set; }
        public double Weight { get; set; } // in pounds
        public DateTime LoggedAt { get; set; }

    }
}
