using Mala3ib.BLL.Contracts.Invitation;

namespace Mala3ib.BLL.Service.Implementation
{
    public class InvitationService : IInvitationService
    {
        private readonly IInvitationRepo _invitationRepo;
        private readonly IFieldSlotRepo _fieldSlotRepo;
        private readonly IPlayerRepo _playerRepo;
        public InvitationService(IInvitationRepo invitationRepo, IFieldSlotRepo fieldSlotRepo, IPlayerRepo playerRepo)
        {
            _invitationRepo = invitationRepo;
            _fieldSlotRepo = fieldSlotRepo;
            _playerRepo = playerRepo;
        }
        private async Task<Result> HandleInvitationAsync(string currentUserId, int invitationId, InvitationStatus newStatus, CancellationToken cancellation)
        {
            var invitation = await _invitationRepo.GetById(invitationId).FirstOrDefaultAsync(cancellation);
            var currentPlayer = await _playerRepo.Get(currentUserId).FirstOrDefaultAsync(cancellation);

            if (invitation is null || invitation.IsDeleted)
                return Result.Failure(InvitationErrors.NotFound);

            if (currentPlayer == null || invitation.RecieverId != currentPlayer.Id)
                return Result.Failure(InvitationErrors.Unauthorized);

            if (invitation.Status != InvitationStatus.Pending)
                return Result.Failure(InvitationErrors.AlreadyHandled);

            invitation.Status = newStatus;

            if (newStatus == InvitationStatus.Accepted)
            {
                var fieldSlot = await _fieldSlotRepo.GetById(invitation.FieldSlotId).FirstOrDefaultAsync(cancellation);
                if (fieldSlot is null)
                    return Result.Failure(FieldSlotErrors.NotFound);

                var joiningPlayerId = invitation.Type == InvitationType.Invite
                    ? invitation.RecieverId
                    : invitation.SenderId;

                var playerAlreadyInSlot = await _fieldSlotRepo.IsPlayerInSlotAsync(fieldSlot.Id, joiningPlayerId, cancellation);
                if (playerAlreadyInSlot)
                    return Result.Failure(InvitationErrors.AlreadyInFieldSlot);

                if (fieldSlot.Players.Count >= fieldSlot.MaxPlayers)
                    return Result.Failure(InvitationErrors.FieldSlotIsFull);

                await _fieldSlotRepo.AddPlayerToSlotAsync(fieldSlot.Id, joiningPlayerId, false, cancellation);

                var updatedSlot = await _fieldSlotRepo.GetById(fieldSlot.Id).FirstOrDefaultAsync(cancellation);
                if (updatedSlot is not null && updatedSlot.Players.Count >= updatedSlot.MaxPlayers)
                    await _fieldSlotRepo.UpdateBookedStatusAsync(fieldSlot.Id, true, cancellation);
            }

            await _invitationRepo.UpdateAsync(invitation, cancellation);

            return Result.Success();
        }
        public async Task<Result> SendAsync(string currentUserId, SendInviationDto request, CancellationToken cancellation = default)
        {
            var fieldSlot = await _fieldSlotRepo.GetById(request.FieldSlotId).FirstOrDefaultAsync(cancellation);
            if (fieldSlot == null)
                return Result.Failure(FieldSlotErrors.NotFound);

            var targetPlayerIsExist = await _playerRepo.IsExistAsync(request.TargetPlayerrId, cancellation);
            if (!targetPlayerIsExist)
                return Result.Failure(PlayerErrors.NotFound);

            if (fieldSlot.Players.Count >= fieldSlot.MaxPlayers)
                return Result.Failure(InvitationErrors.FieldSlotIsFull);

            var targetAlreadyInSlot = await _fieldSlotRepo.IsPlayerInSlotAsync(request.FieldSlotId, request.TargetPlayerrId, cancellation);
            if (targetAlreadyInSlot)
                return Result.Failure(InvitationErrors.AlreadyInFieldSlot);

            var captain = fieldSlot.Players
                .FirstOrDefault(p => p.IsCaptain)?.Player;

            var currentPlayer = await _playerRepo.Get(currentUserId).FirstOrDefaultAsync(cancellation);
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
            var player = await _playerRepo.Get(userId).FirstOrDefaultAsync(cancellation);
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
            var fieldSlot = await _fieldSlotRepo.GetById(fieldSlotId).FirstOrDefaultAsync(cancellation);
            if (fieldSlot == null)
                return Result.Failure(FieldSlotErrors.NotFound);

            var captain = fieldSlot.Players
                .FirstOrDefault(p => p.IsCaptain)?.Player;
            var currentPlayer = await _playerRepo.Get(currentUserId).FirstOrDefaultAsync(cancellation);

            if (currentPlayer == null || captain == null)
                return Result.Failure(InvitationErrors.Unauthorized);

            if (currentPlayer.Id == captain.Id)
                return Result.Failure(InvitationErrors.CannotInviteYourself);

            if (fieldSlot.Players.Count >= fieldSlot.MaxPlayers)
                return Result.Failure(InvitationErrors.FieldSlotIsFull);

            var currentPlayerInSlot = await _fieldSlotRepo.IsPlayerInSlotAsync(fieldSlotId, currentPlayer.Id, cancellation);
            if (currentPlayerInSlot)
                return Result.Failure(InvitationErrors.AlreadyInFieldSlot);

            var result = await _invitationRepo.InviteAsync(currentPlayer.Id, captain.Id, fieldSlotId, InvitationType.Request, cancellation);

            if (!result)
                return Result.Failure(InvitationErrors.AlreadyRequested);

            return Result.Success();
        }

        public async Task<Result<IEnumerable<InvitationResponseDto>>> GetSentInvitations(string currentUserId, InvitationStatus status, CancellationToken cancellation = default)
        {
            var player = await _playerRepo.Get(currentUserId).FirstOrDefaultAsync(cancellation);
            if (player == null)
                return Result.Failure<IEnumerable<InvitationResponseDto>>(PlayerErrors.NotFound);

            var invitations = await _invitationRepo.GetSentInvitations(player.Id, status)
                .Select(i => new InvitationResponseDto(
                    i.SenderId,
                    i.RecieverId,
                    i.FieldSlotId,
                    i.Status,
                    i.Type
                    )).ToListAsync(cancellation);

            return Result.Success<IEnumerable<InvitationResponseDto>>(invitations);
        }

        public async Task<Result<IEnumerable<InvitationResponseDto>>> GetRecievedInvitations(string currentUserId, InvitationStatus status, CancellationToken cancellation = default)
        {
            var player = await _playerRepo.Get(currentUserId).FirstOrDefaultAsync(cancellation);
            if (player == null)
                return Result.Failure<IEnumerable<InvitationResponseDto>>(PlayerErrors.NotFound);

            var invitations = await _invitationRepo.GetReceivedInvitations(player.Id, status)
                .Select(i => new InvitationResponseDto(
                    i.SenderId,
                    i.RecieverId,
                    i.FieldSlotId,
                    i.Status,
                    i.Type
                    )).ToListAsync(cancellation);

            return Result.Success<IEnumerable<InvitationResponseDto>>(invitations);
        }
    }
}