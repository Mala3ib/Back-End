namespace Mala3ib.DAL.Entities
{
    public class Invitation
    {
        public int Id { get; set; }
        public InvitationStatus Status { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public ApplicationUser Sender { get; set; } = default!;
        public string RecieverId { get; set; } = string.Empty;
        public ApplicationUser Reciever { get; set; } = default!;
        public int FieldSlotId { get; set; }
        public FieldSlot FieldSlot { get; set; } = default!;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
    }
}