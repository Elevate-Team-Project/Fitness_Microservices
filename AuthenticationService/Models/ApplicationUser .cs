using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? ProfileImageUrl { get; set; }
    
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string FullName { get; set; }
     
        public double Height { get; set; }  // in centimeters
        public double Weight { get; set; }  // in kilograms

        public int Age { get; set; }
        public string Gender { get; set; }

        public string ActivtyLevel { get; set; }

        public string Goal { get; set; }



        // 🔁 Refresh Token Support
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
