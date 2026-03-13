using Mala3ib.DAL.Enums;

namespace Mala3ib.DAL.Entities;

public class Match
{
    public int Id { get; set; }
    public int CreatorPlayerId { get; set; }
    public DateTime Date { get; set; }
    public int MaxPlayers { get; set; }
    public int CurrentPlayers { get; set; }
    public bool IsDeleted { get; set; } = false;
    public MatchStatus Status { get; set; }
    public Player Player { get; set; } = default!;
    public int FieldId { get; set; }
    public Field Field { get; set; } = default!;
    public int FieldSlotId { get; set; }
    public FieldSlot FieldSlot { get; set; } = default!;
    public ICollection<Booking> Bookings { get; set; } =  new List<Booking>();
    public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
}