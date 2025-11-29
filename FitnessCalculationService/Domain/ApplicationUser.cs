using Microsoft.AspNetCore.Identity;

namespace Fitness.Data
{
    public class ApplicationUser : IdentityUser
    {

        public string ?Address { get; set; }

        public string Email { get; set; }

    }
}
