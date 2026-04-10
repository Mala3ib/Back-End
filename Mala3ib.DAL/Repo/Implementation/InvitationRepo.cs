namespace Mala3ib.DAL.Repo.Implementation
{
    public class InvitationRepo : IInvitationRepo
    {
        private readonly ApplicationDbContext _context;
        public InvitationRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> InviteAsync(string myUserId, string targetUserId, int fieldSlotId, CancellationToken cancellation)
        {
            var invitation = await _context.Invitations
                .FirstOrDefaultAsync(x => x.SenderId == myUserId && x.RecieverId == targetUserId && x.FieldSlotId == fieldSlotId, cancellation);

            if (invitation is not null)
            {
                if (!invitation.IsDeleted)
                    return false;

                invitation.IsDeleted = false;
                invitation.Status = InvitationStatus.Pending;
                invitation.CreatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellation);
                return true;
            }

            var newInviation = new Invitation
            {
                SenderId = myUserId,
                RecieverId = targetUserId,
                FieldSlotId = fieldSlotId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Invitations.AddAsync(newInviation);
            await _context.SaveChangesAsync(cancellation);

            return true;
        }

        public async Task<bool> ExistsAsync(string receiverId, int invitationId, CancellationToken cancellation = default)
        {
            return await _context.Invitations
                .AnyAsync(x => x.RecieverId == receiverId && x.Id == invitationId && !x.IsDeleted, cancellation);
        }

        public IQueryable<Invitation> GetById(int id)
        {
            var invitation = _context.Invitations
                .Where(x => x.Id == id && !x.IsDeleted)
                .AsNoTracking();

            return invitation;
        }

        public IQueryable<Invitation> GetReceivedInvitations(string id)
        {
            var invitation = _context.Invitations
                .Where(x => x.RecieverId == id && !x.IsDeleted)
                .AsNoTracking();

            return invitation;
        }

        public IQueryable<Invitation> GetSentInvitations(string id)
        {
            var invitation = _context.Invitations
                .Where(x => x.SenderId == id && !x.IsDeleted)
                .AsNoTracking();

            return invitation;
        }

        public async Task UpdateAsync(Invitation invitation, CancellationToken cancellation = default)
        {
            await _context.Invitations
                .Where(f => f.Id == invitation.Id)
                .ExecuteUpdateAsync(setter =>
                    setter.SetProperty(x => x.Status, invitation.Status), cancellation
                );
        }

        public async Task DeleteAsync(int id, CancellationToken cancellation = default)
        {
            var affectedRows = await _context.Invitations
                .Where(x => x.Id == id && !x.IsDeleted)
                .ExecuteUpdateAsync(setter =>
                setter.SetProperty(x => x.IsDeleted, true)
                );
        }
    }
}