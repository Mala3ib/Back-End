namespace Mala3ib.BLL.Service.Abstraction
{
    public interface IEmailVerificationService
    {
        Task SendOtpAsync(ApplicationUser user);
        Task<Result> VerifyEmailAsync(ApplicationUser user, string code);
    }
}
