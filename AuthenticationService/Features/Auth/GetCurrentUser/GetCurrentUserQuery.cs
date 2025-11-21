using MediatR;
using AuthenticationService.Features.Auth.Login;

namespace AuthenticationService.Features.Auth.GetCurrentUser
{
    public record GetCurrentUserQuery(string UserId) : IRequest<LoginResponse>;

}
