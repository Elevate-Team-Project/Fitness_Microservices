using Fitness.Data.Enums;

namespace Fitness.Features.Dtos
{
    public class AddWGA
    {

        public Guid UserId { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public ActivityLevel ActivityLevel { get; set; }
        public Goal Goal { get; set; }






    }
}
