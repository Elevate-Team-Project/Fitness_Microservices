namespace AuthenticationService.Dtos
{
    public class WgaDto
    {
        public Guid UserId { get; set; }
        public double Height { get; set; }  // in centimeters
        public double Weight { get; set; }  // in kilograms

        public int Age { get; set; }
        public string Gender { get; set; }

        public string ActivtyLevel { get; set; }

        public string Goal { get; set; }

    }
}
