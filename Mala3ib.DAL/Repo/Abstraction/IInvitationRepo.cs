namespace Mala3ib.DAL.Repo.Abstraction
{
    public interface IInvitationRepo
    {
        Task<bool> InviteAsync(int sendPlayerId, int targetPlayerId, int fieldSlotId, InvitationType type, CancellationToken cancellation);
        IQueryable<Invitation> GetReceivedInvitations(int id, InvitationStatus status);
        IQueryable<Invitation> GetSentInvitations(int id, InvitationStatus status);
        IQueryable<Invitation> GetById(int id);
        Task DeleteAsync(int id, CancellationToken cancellation = default);
        Task UpdateAsync(Invitation invitation, CancellationToken cancellation = default);
    }
}