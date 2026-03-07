namespace Mala3ib.BLL.Service.Implementation
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailService;
        private readonly EmailBodyBuilder _emailBodyBuilder;
        public EmailVerificationService(ApplicationDbContext context,
                IEmailSender emailService,
                EmailBodyBuilder emailBodyBuilder)
        {
            _context = context;
            _emailService = emailService;
            _emailBodyBuilder = emailBodyBuilder;
        }

        public async Task SendOtpAsync(ApplicationUser user)
        {
            var otp = GenerateOtp();

            var userOtps = new EmailVerficationOtp
            {
                UserId = user.Id,
                Code = otp,
                ExpirationTime = DateTime.UtcNow.AddMinutes(10)
            };

            await SendEmail(user, userOtps.Code);

            await _context.EmailVerficationOtps.AddAsync(userOtps);
            await _context.SaveChangesAsync();
        }

        public async Task<Result> VerifyEmailAsync(ApplicationUser user, string code)
        {
            var userOtp = await _context.EmailVerficationOtps
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.ExpirationTime)
                .FirstOrDefaultAsync();

            if (userOtp is null)
                return Result.Failure(new Error("Invalid.Otp", "Invalid verification code", StatusCodes.Status401Unauthorized));

            if (userOtp.IsUsed)
                return Result.Failure(new Error("Invalid.Otp", "This verification code has already been used.", StatusCodes.Status401Unauthorized));
            
            if (userOtp.ExpirationTime < DateTime.UtcNow)
                return Result.Failure(new Error("Invalid.Otp", "The verification code has expired.", StatusCodes.Status401Unauthorized));

            if(userOtp.Code != code)
                return Result.Failure(new Error("Invalid.Otp", "The verification code is incorrect.", StatusCodes.Status401Unauthorized));

            user.EmailConfirmed = true;
            userOtp.IsUsed = true;

            await _context.SaveChangesAsync();
            return Result.Success();
        }

        private string GenerateOtp()
        {
            var random = new Random();

            return random.Next(100000, 999999).ToString();
        }

        private async Task SendEmail(ApplicationUser user, string otp)
        {

            var emailBody = _emailBodyBuilder.GenerateEmailBody("EmailConfirmation.html",
                new Dictionary<string, string>
                {
                        { "{{UserName}}" , $"{user.FirstName} {user.LastName}"},
                        { "{{OTP_CODE}}", $"{otp}" }
                }
            );

            await _emailService.SendEmailAsync(user.Email!, "✅ Mala3ib Email Confirmation", emailBody);
            await Task.CompletedTask;
        }
    }
}
