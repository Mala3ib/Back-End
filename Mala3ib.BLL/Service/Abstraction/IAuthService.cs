using Mala3ib.BLL.Contracts.Authentication;
using Mala3ib.DAL.Abstraction;

namespace Mala3ib.BLL.Service.Abstraction
{
    public interface IAuthService
    {
        Task <Result<AuthResponse>?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    }
}
