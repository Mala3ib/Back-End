namespace Mala3ib.BLL.Contracts.FieldSlot
{
    public record FieldSlotResponseDto(
        DateTime StartTime,
        DateTime EndTime,
        bool IsBooked
    );
}