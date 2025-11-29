using FitnessCalculationService.Domain.Enums;

namespace Fitness.Data
{
    public class WeightGoalActivitydb:BaseEntity
    {
        public Guid Id { get; set; }

        public Guid  UserId { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public double Weight { get; set; }
        public double Height { get; set; }
        public ActivityLevel ActivityLevel { get; set; }
        public Goal Goal { get; set; }

        public DateTime InsertDate { get; set; } = DateTime.Now;

        public UserFitnessStatdb FitnessStat { get; set; }

    }
}
