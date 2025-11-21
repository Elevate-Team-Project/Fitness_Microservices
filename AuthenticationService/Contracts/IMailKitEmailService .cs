namespace AuthenticationService.Contarcts
{
    public interface IMailKitEmailService
    {
       
        Task SendEmailAsync(string toEmail, string subject, string body);

    }
}
