namespace AuthenticationService.Features.Auth.Register
{
    public class RegisterDto
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
        public double Height { get; set; }  // in centimeters
        public double Weight { get; set; }  // in kilograms
        public int Age { get; set; }
        public string Gender { get; set; }
        public string ActivtyLevel { get; set; }
        public string Goal { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
