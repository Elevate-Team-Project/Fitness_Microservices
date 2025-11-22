namespace ProgressTrackingService.Feature.Waight.UpdateWaightGoal.DTOs
{
    public class UpdateGoalWaightDtoResponse
    {
        int ? id;
        public int UserId { get; set; }
        public double NewGoalWeight { get; set; }
    }
}
