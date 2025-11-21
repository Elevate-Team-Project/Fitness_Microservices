using MediatR;

namespace AuthenticationService.Features.Auth.Logout
{
    public record LogoutCommand(Guid UserId) : IRequest<LogoutResponse>;

}
