using AuthenticationService.Features.Auth.UpdateUserProfile;
using MediatR;

public record UpdateUserProfileCommand(
    Guid UserId,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? ProfileImage,
    string? Goal,
    string? activtyLevel,
    double Height,
    double Weight
   
) : IRequest<UpdateUserProfileResponse>;
