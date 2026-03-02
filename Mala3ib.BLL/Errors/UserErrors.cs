using Mala3ib.DAL.Abstraction;
using Microsoft.AspNetCore.Http;

namespace Mala3ib.BLL.Errors
{
    public static class UserErrors
    {
        public static Error InvalidCredentials
            = new Error("User.InvalidCredentials", "Invalid email or password", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidTokens
           = new Error("User.InvalidCredentials", "Invalid token or resfresh token", StatusCodes.Status401Unauthorized);

    }
}
