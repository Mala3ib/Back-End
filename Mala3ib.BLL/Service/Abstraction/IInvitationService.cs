using Mala3ib.BLL.Contracts.Invitation;

namespace Mala3ib.BLL.Service.Abstraction
{
    public interface IInvitationService
    {
        Task<Result> SendAsync(string currentUserId, SendInviationDto request, CancellationToken cancellation = default);
        Task<Result> AcceptAsync(string currentUserId, int invitationId, CancellationToken cancellation = default);
        Task<Result> RejectAsync(string currentUserId, int invitationId, CancellationToken cancellation = default);
        Task<Result> DeleteAsync(int id, string userId, CancellationToken cancellation = default);
        // Join Request
    }
}