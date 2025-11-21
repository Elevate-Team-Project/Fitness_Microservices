using MediatR;

namespace AuthenticationService.Features.Auth.ForgetPassword.OTP
{
    public record SendOtpCommand(string Email) : IRequest<bool>;

}
