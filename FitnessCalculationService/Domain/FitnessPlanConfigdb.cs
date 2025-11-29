using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Fitness.Data
{
    public class FitnessPlanConfigdb:BaseEntity
    {
  
        public Guid Id { get; set; }

        [Required, MaxLength(20)]
        public string Goal { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string Status { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal MinCalorie { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal MaxCalorie { get; set; }

       
        public int WorkoutPlanId { get; set; }

        [Required, MaxLength(20)]
        public string PlanType { get; set; } = "Combined"; 

        public WorkoutPlandb? WorkoutPlan { get; set; }


    }
}

