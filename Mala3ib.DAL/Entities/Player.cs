
namespace Mala3ib.DAL.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? Image { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = default!;
        public ICollection<Match> CreatedMatches { get; set; } = new List<Match>();
        public ICollection<Booking> Bookings { get; set; }  = new List<Booking>();
        public ICollection<Invitation> SentInvitations { get; set; }  = new List<Invitation>();
        public ICollection<Invitation> ReceivedInvitations { get; set; }  = new List<Invitation>();
        public ICollection<FieldReview> FieldReviews { get; set; } = new List<FieldReview>();
    }
}