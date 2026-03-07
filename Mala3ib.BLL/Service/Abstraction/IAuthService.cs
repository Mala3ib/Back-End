using Microsoft.AspNetCore.Identity.Data;

namespace Mala3ib.BLL.Service.Abstraction
{
    public interface IAuthService
    {
        Task <Result<AuthResponseDto>?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<Result<AuthResponseDto>?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
        Task<Result> ConfirmEmailAsync(ConfirmEmailRequestDto request);
        Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequestDto request);
    }
}
