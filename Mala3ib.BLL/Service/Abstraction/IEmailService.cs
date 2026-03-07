namespace Mala3ib.BLL.Service.Abstraction
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
