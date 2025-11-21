using MediatR;

namespace AuthenticationService.Features.Auth.Register
{
    public record RegisterCommand(RegisterDto RegisterDto) : IRequest<RegisterResponse>;

}
