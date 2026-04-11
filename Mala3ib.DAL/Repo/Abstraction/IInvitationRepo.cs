namespace Mala3ib.DAL.Repo.Abstraction
{
    public interface IInvitationRepo
    {
        Task<bool> InviteAsync(int sendPlayerId, int targetPlayerId, int fieldSlotId, InvitationType type, CancellationToken cancellation);
        Task<bool> ExistsAsync(int receiverId, int invitationId, CancellationToken cancellation = default);
        IQueryable<Invitation> GetReceivedInvitations(int id);
        IQueryable<Invitation> GetSentInvitations(int id);
        IQueryable<Invitation> GetById(int id);
        Task DeleteAsync(int id, CancellationToken cancellation = default);
        Task UpdateAsync(Invitation invitation, CancellationToken cancellation = default);
    }
}