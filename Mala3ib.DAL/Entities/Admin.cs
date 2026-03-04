
using Mala3ib.DAL.Enums;

namespace Mala3ib.DAL.Entities
{
    public class Admin
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = default!;

        public string? NationalId { get; set; }
    }
}