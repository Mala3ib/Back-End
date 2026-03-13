using Mala3ib.DAL.Enums;

namespace Mala3ib.DAL.Entities;

public class Booking
{
    public int Id { get; set; }
    public BookingStatus Status { get; set; }
    public bool IsDeleted { get; set; } = false;
    public int MatchId { get; set; }
    public Match Match { get; set; } = default!;
    public int PlayerId { get; set; }
    public Player Player { get; set; } = default!;
    public DateTime BookingTime { get; set; }
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}