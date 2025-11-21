namespace AuthenticationService.Features.Auth.UpdateUserProfile
{
    public record UpdateUserProfileRequest
 (
     string? FirstName,
     string? LastName,
     string? PhoneNumber,
       string Goal,
     string activtyLevel,
        double Weight,
        double Height,
     IFormFile? ProfileImage
   


 );
}
