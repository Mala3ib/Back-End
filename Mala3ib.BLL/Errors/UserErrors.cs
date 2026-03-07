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

        public static readonly Error DuplicatedEmail
            = new Error("User.DuplicatedEmail", "Another user with the same emial is already exists", StatusCodes.Status409Conflict);

        public static readonly Error EmailNotConfirmed
           = new Error("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidCode
        = new Error("User.InvalidCode", "Invalid code", StatusCodes.Status401Unauthorized);

        public static readonly Error DuplicatedConfirmation
                = new Error("User.DuplicatedConfirmation", "Email is already confirmed", StatusCodes.Status400BadRequest);
    }
}
