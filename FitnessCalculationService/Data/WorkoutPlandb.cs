using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Fitness.Data
{
    public class WorkoutPlandb:BaseEntity
    {
 
        public Guid PlanId { get; set; }

        [Required, MaxLength(100)]
        public string PlanName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DurationWeeks { get; set; }

        [MaxLength(10)]
        public string Difficulty { get; set; } = string.Empty; // Weak, Normal, Hard

        public ICollection<FitnessPlanConfigdb>? FitnessPlanConfigs { get; set; }
    }
}
