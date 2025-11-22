namespace Fitness.Data
{
    public class UserFitnessStatdb:BaseEntity
    {

        public Guid Id { get; set; }


        public Guid UserId { get; set; }

        public int weightGoalActivityId { get; set; }

        public decimal Bmr { get; set; }

        public decimal Tdee { get; set; }

        public decimal CalorieTarget { get; set; }

        public string Status { get; set; } 

        public DateTime InsertDate { get; set; } = DateTime.Now;

        public WeightGoalActivitydb weightGoalActivity { get; set; }
    }
}

