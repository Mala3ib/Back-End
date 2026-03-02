
using Microsoft.AspNetCore.Identity;

namespace Mala3ib.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string  FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}