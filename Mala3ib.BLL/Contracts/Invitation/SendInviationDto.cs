namespace Mala3ib.BLL.Contracts.Invitation
{
    public record SendInviationDto(
        string TargetUserId,
        int FieldSlotId
    );
}