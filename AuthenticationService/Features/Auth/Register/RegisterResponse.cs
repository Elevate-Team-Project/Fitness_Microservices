namespace AuthenticationService.Features.Auth.Register
{
    public record RegisterResponse(
         bool Success,
         string Message,
         Guid UserId,
         string UserName,
         string FirstName,
         string LastName,
         string FullName,
         string PhoneNumber,
         string Email,
         string ProfileImageUrl,
            double Height,  // in centimeters\\
            double Weight,  // in kilograms\\
            int Age,
            string ActivtyLevel,
            string Goal,
            string Gender,
         IList<string> Roles ,
         string Token,
         string? RefreshToken

        );
}
