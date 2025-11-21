using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string? Description { get; set; }
    }
}
