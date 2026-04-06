namespace Mala3ib.BLL.Contracts.FieldSlot
{
    public record FieldSlotResponseDto(
        DateTime StartTime,
        DateTime EndTime,
        decimal Price,
        int MaxPlayers,
        int InvitedPlayers,
        bool IsBooked
    );
}