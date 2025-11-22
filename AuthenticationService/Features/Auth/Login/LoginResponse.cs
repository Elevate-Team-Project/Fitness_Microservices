namespace AuthenticationService.Features.Auth.Login
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string lastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public double Height { get; set; }  // in centimeters
        public double Weight { get; set; }  // in kilograms
        public int Age { get; set; }
        public string Gender { get; set; }

        public string ActivtyLevel { get; set; }

        public string Goal { get; set; }
        public IEnumerable<string>? Roles { get; set; }
        public string Token { get; set; }
        public string? RefreshToken { get; set; }

    }
}
