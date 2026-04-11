using Mala3ib.BLL.Contracts.Invitation;

namespace Mala3ib.BLL.Service.Implementation
{
    public class InvitationService : IInvitationService
    {
        private readonly IInvitationRepo _invitationRepo;
        private readonly IFieldSlotRepo _fieldSlotRepo;
        private readonly IPlayerRepo _playerRepo;
        public InvitationService(IInvitationRepo invitationRepo, IUserRepo userRepo, IFieldSlotRepo fieldSlotRepo, IPlayerRepo playerRepo)
        {
            _invitationRepo = invitationRepo;
            _fieldSlotRepo = fieldSlotRepo;
            _playerRepo = playerRepo;
        }
        private async Task<Result> HandleInvitationAsync(string currentUserId, int invitationId, InvitationStatus newStatus, CancellationToken cancellation)
        {
            var invitation = _invitationRepo.GetById(invitationId).FirstOrDefault();
            var currentPlayer = _playerRepo.Get(currentUserId).FirstOrDefault();

            if (invitation is null || invitation.IsDeleted)
                return Result.Failure(InvitationErrors.NotFound);

            if (currentPlayer == null || invitation.RecieverId != currentPlayer.Id)
                return Result.Failure(InvitationErrors.Unauthorized);

            if (invitation.Status != InvitationStatus.Pending)
                return Result.Failure(InvitationErrors.AlreadyHandled);

            invitation.Status = newStatus;

            await _invitationRepo.UpdateAsync(invitation, cancellation);

            return Result.Success();
        }
        public async Task<Result> SendAsync(string currentUserId, SendInviationDto request, CancellationToken cancellation = default)
        {
            var fieldSlot = _fieldSlotRepo.GetById(request.FieldSlotId).FirstOrDefault();
            if (fieldSlot == null)
                return Result.Failure(FieldSlotErrors.NotFound);

            var targetPlayerIsExist = await _playerRepo.IsExistAsync(request.TargetPlayerrId);
            if (!targetPlayerIsExist)
                return Result.Failure(PlayerErrors.NotFound);

            var captain = fieldSlot.Players
                .FirstOrDefault(p => p.IsCaptain)?.Player;

            var currentPlayer = _playerRepo.Get(currentUserId).FirstOrDefault();
            if (currentPlayer == null || captain == null || captain.Id != currentPlayer.Id)
                return Result.Failure(InvitationErrors.Unauthorized);

            if (currentPlayer.Id == request.TargetPlayerrId)
                return Result.Failure(InvitationErrors.CannotInviteYourself);

            var result = await _invitationRepo.InviteAsync(currentPlayer.Id, request.TargetPlayerrId, request.FieldSlotId, InvitationType.Invite, cancellation);

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
            var player = _playerRepo.Get(userId).FirstOrDefault();
            if (player == null)
                return Result.Failure(InvitationErrors.Unauthorized);

            var invitation = await _invitationRepo.GetById(id)
                .FirstOrDefaultAsync(cancellation);

            if (invitation is null)
                return Result.Failure(InvitationErrors.NotFound);

            if (invitation.SenderId != player.Id)
                return Result.Failure(InvitationErrors.Unauthorized);

            if (invitation.Status != InvitationStatus.Pending)
                return Result.Failure(InvitationErrors.AlreadyHandled);

            await _invitationRepo.DeleteAsync(id, cancellation);
            return Result.Success();
        }

        public async Task<Result> RequestAsync(string currentUserId, int fieldSlotId, CancellationToken cancellation = default)
        {
            var fieldSlot = _fieldSlotRepo.GetById(fieldSlotId).FirstOrDefault();
            if (fieldSlot == null)
                return Result.Failure(FieldSlotErrors.NotFound);

            var captain = fieldSlot.Players
                .FirstOrDefault(p => p.IsCaptain)?.Player;
            var currentPlayer = _playerRepo.Get(currentUserId).FirstOrDefault();

            if (currentPlayer == null || captain == null)
                return Result.Failure(InvitationErrors.Unauthorized);

            if (currentPlayer.Id == captain.Id)
                return Result.Failure(InvitationErrors.CannotInviteYourself);

            if (fieldSlot.Players.Count >= fieldSlot.MaxPlayers)
                return Result.Failure(InvitationErrors.FieldSlotIsFull);

            var result = await _invitationRepo.InviteAsync(currentPlayer.Id, captain.Id, fieldSlotId, InvitationType.Request, cancellation);

            if (!result)
                return Result.Failure(InvitationErrors.AlreadyRequested);

            return Result.Success();
        }
    }
}