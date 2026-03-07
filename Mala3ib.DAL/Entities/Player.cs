
namespace Mala3ib.DAL.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? Image { get; set; } 
        
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = default!;
    }
}