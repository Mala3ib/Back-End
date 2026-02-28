
using Mala3ib.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Mala3ib.DAL.Database
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
    }
}