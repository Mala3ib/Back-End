using Mala3ib.BLL.Authentication;
using Mala3ib.BLL.Contracts.Authentication;
using Mala3ib.BLL.Errors;
using Mala3ib.DAL.Abstraction;
using Mala3ib.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Mala3ib.BLL.Service.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IJwtProvider _jwtProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthService(IJwtProvider jwtProvider, UserManager<ApplicationUser> userManager)
        {
            _jwtProvider = jwtProvider;
            _userManager = userManager;
        }

        public async Task<Result<AuthResponse>?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

            var isValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!isValidPassword)
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

            var (token, expiresIn) = _jwtProvider.GenerateToken(user);

            return Result.Success(new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn));
        }
    }
}
