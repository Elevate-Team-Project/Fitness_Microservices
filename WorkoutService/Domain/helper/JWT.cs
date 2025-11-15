namespace WorkoutService.Domain.helper
{
    public class JWT
    {

        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string SecretKey { get; set; }
        public int ExpiryInMinutes { get; set; }

    }
}
