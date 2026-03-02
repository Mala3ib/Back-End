using Mala3ib.DAL.Entities;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mala3ib.BLL.Authentication
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _jwtOptions;

        public JwtProvider(IOptions<JwtOptions> option)
        {
            _jwtOptions = option.Value;
        }

        public (string token, int expiresIn) GenerateToken(ApplicationUser user)
        {
            Claim[] claims = [
                new (JwtRegisteredClaimNames.Sub, user.Id),
                new (JwtRegisteredClaimNames.Email, user.Email!),
                new (JwtRegisteredClaimNames.GivenName, user.FirstName),
                new (JwtRegisteredClaimNames.FamilyName, user.LastName),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ];

            var symmeticSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var signingCredentials = new SigningCredentials(symmeticSecurityKey, SecurityAlgorithms.HmacSha256);

            var expiresIn = _jwtOptions.ExpiryMinutes;
            var expirationDate = DateTime.UtcNow.AddMinutes(expiresIn);

            var token = new JwtSecurityToken(
                    issuer: _jwtOptions.Issuer,
                    audience: _jwtOptions.Audience,
                    claims: claims,
                    expires: expirationDate,
                    signingCredentials: signingCredentials
            );

            return (token: new JwtSecurityTokenHandler().WriteToken(token), expiresIn: expiresIn * 60);
        }
    }
}
