using Mala3ib.BLL.Contracts.Invitation;

namespace Mala3ib.BLL.Service.Implementation
{
    public class InvitationService : IInvitationService
    {
        private readonly IInvitationRepo _invitationRepo;
        private readonly IFieldSlotRepo _fieldSlotRepo;
        private readonly IUserRepo _userRepo;
        public InvitationService(IInvitationRepo invitationRepo, IUserRepo userRepo, IFieldSlotRepo fieldSlotRepo)
        {
            _invitationRepo = invitationRepo;
            _userRepo = userRepo;
            _fieldSlotRepo = fieldSlotRepo;
        }
        private async Task<Result> HandleInvitationAsync(string currentUserId, int invitationId, InvitationStatus newStatus, CancellationToken cancellation)
        {
            var invitation = _invitationRepo.GetById(invitationId).FirstOrDefault();

            if (invitation is null || invitation.IsDeleted)
                return Result.Failure(InvitationErrors.NotFound);

            if (invitation.RecieverId != currentUserId)
                return Result.Failure(InvitationErrors.Unauthorized);

            if (invitation.Status != InvitationStatus.Pending)
                return Result.Failure(InvitationErrors.AlreadyHandled);

            invitation.Status = newStatus;

            await _invitationRepo.UpdateAsync(invitation, cancellation);

            return Result.Success();
        }
        public async Task<Result> SendAsync(string currentUserId, SendInviationDto request, CancellationToken cancellation = default)
        {
            var targetUserIsExist = await _userRepo.IsExistAsync(request.TargetUserId, cancellation);
            if (!targetUserIsExist)
                return Result.Failure(UserErrors.NotFouond);

            if (currentUserId == request.TargetUserId)
                return Result.Failure(InvitationErrors.CannotInviteYourself);

            var fieldSlotIsExist = await _fieldSlotRepo.IsExist(request.FieldSlotId, cancellation);
            if (!fieldSlotIsExist)
                return Result.Failure(FieldSlotErrors.NotFound);

            var result = await _invitationRepo.InviteAsync(currentUserId, request.TargetUserId, request.FieldSlotId, cancellation);

            if (!result)
                return Result.Failure(InvitationErrors.AlreadySent);

            return Result.Success();
        }
        public async Task<Result> AcceptAsync(string currentUserId, int invitationId, CancellationToken cancellation = default)
        {
            return await HandleInvitationAsync(currentUserId, invitationId, InvitationStatus.Accepted, cancellation);
        }
        public async Task<Result> RejectAsync(string currentUserId, int invitationId, CancellationToken cancellation = default)
        {
            return await HandleInvitationAsync(currentUserId, invitationId, InvitationStatus.Rejected, cancellation);
        }

        public async Task<Result> DeleteAsync(int id, string userId, CancellationToken cancellation = default)
        {
            var invitation = await _invitationRepo.GetById(id)
                .FirstOrDefaultAsync(cancellation);

            if (invitation is null)
                return Result.Failure(InvitationErrors.NotFound);

            if (invitation.SenderId != userId)
                return Result.Failure(InvitationErrors.Unauthorized);

            if (invitation.Status != InvitationStatus.Pending)
                return Result.Failure(InvitationErrors.AlreadyHandled);

            await _invitationRepo.DeleteAsync(id, cancellation);
            return Result.Success();
        }
    }
}