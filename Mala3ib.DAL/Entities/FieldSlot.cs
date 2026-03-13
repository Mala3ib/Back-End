namespace Mala3ib.DAL.Entities
{
    public class FieldSlot
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsBooked { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public int FieldId { get; set; }
        public Field Field { get; set; } = default!;
    }
}