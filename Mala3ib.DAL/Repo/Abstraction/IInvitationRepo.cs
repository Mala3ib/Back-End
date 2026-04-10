namespace Mala3ib.DAL.Repo.Abstraction
{
    public interface IInvitationRepo
    {
        Task<bool> InviteAsync(string myUserId, string targetUserId, int fieldSlotId, CancellationToken cancellation);
        Task<bool> ExistsAsync(string receiverId, int invitationId, CancellationToken cancellation = default);

        IQueryable<Invitation> GetReceivedInvitations(string id);
        IQueryable<Invitation> GetSentInvitations(string id);
        IQueryable<Invitation> GetById(int id);
        Task DeleteAsync(int id, CancellationToken cancellation = default);
        Task UpdateAsync(Invitation invitation, CancellationToken cancellation = default);
    }
}