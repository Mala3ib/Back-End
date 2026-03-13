using Mala3ib.DAL.Enums;
using Microsoft.AspNetCore.Identity;

namespace Mala3ib.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public Role Role { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; } = [];
        public List<EmailVerficationOtp> EmailVerficationOtps = [];
        public Player Player { get; set; } = default!;
        public FieldOwner FieldOwner { get; set; } = default!;
        public Admin Admin { get; set; } = default!;
    }
}