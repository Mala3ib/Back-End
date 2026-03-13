using Mala3ib.DAL.Enums;

namespace Mala3ib.DAL.Entities;

public class Payment
{
    public int Id { get; set; }
    public Decimal Amount { get; set; }
    public bool IsDeleted { get; set; } = false;
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = default!;
}