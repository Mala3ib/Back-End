using System.ComponentModel.DataAnnotations.Schema;
using Mala3ib.DAL.Enums;

namespace Mala3ib.DAL.Entities;

public class Invitation
{
    public int Id { get; set; }
    public InvitationStatus Status { get; set; }
    public bool IsDeleted { get; set; } = false;
    public int MatchId { get; set; }
    public Match Match { get; set; } = default!;
    [ForeignKey("Sender")]
    public int SenderId { get; set; }
    public Player Sender { get; set; } = default!;
    [ForeignKey("Receiver")]
    public int ReceiverId { get; set; }
    public Player Receiver { get; set; } = default!;
}