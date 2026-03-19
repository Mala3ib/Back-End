namespace Mala3ib.DAL.Repo.Implementation
{
    public class FollowRepo : IFollowRepo
    {
        private readonly ApplicationDbContext _context;

        public FollowRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result> FollowAsync(string myUserId, string targetUserId, CancellationToken cancellation)
        {
            var targetExists = await _context.Users
                .AnyAsync(x => x.Id == targetUserId && !x.IsDeleted, cancellation);

            if (!targetExists)
                return Result.Failure(UserErrors.NotFouond);

            var follow = await _context.Follows
                .FirstOrDefaultAsync(x => x.FollowerId == myUserId && x.FollowingId == targetUserId, cancellation);

            if (follow is not null)
            {
                if (!follow.IsDeleted)
                    return Result.Failure(FollowErrors.AlreadyFollowing);

                // Restore soft deleted

                follow.IsDeleted = false;
                follow.CreatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellation);
                return Result.Success();
            }

            var newFollow = new Follow
            {
                FollowerId = myUserId,
                FollowingId = targetUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Follows.Add(newFollow);
            await _context.SaveChangesAsync(cancellation);

            return Result.Success();
        }
        public async Task<Result> UnFollowAsync(string myUserId, string targetUserId, CancellationToken cancellation)
        {
            var targetExists = await _context.Users
                .AnyAsync(x => x.Id == targetUserId && !x.IsDeleted, cancellation);

            if (!targetExists)
                return Result.Failure(PlayerErrors.NotFound);

            var follow = await _context.Follows
                .FirstOrDefaultAsync(x => x.FollowerId == myUserId && x.FollowingId == targetUserId, cancellation);

            if (follow is not null)
            {
                if (follow.IsDeleted)
                    return Result.Failure(FollowErrors.AlreadyUnfollowed);

                follow.IsDeleted = true;

                await _context.SaveChangesAsync(cancellation);
                return Result.Success();
            }

            return Result.Failure(FollowErrors.AlreadyUnfollowed);
        }

        public async Task<Result<IQueryable<ApplicationUser>>> GetFollowing(string userId)
        {
            var isExist = await _context.Users.AnyAsync(x => x.Id == userId);
            if (!isExist)
                return Result.Failure<IQueryable<ApplicationUser>>(UserErrors.NotFouond);
            
            return Result.Success(_context.Follows
                .Where(x => x.FollowerId == userId && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => x.Following)
                .AsNoTracking());
        }

        public async Task<Result<IQueryable<ApplicationUser>>> GetFollowers(string userId)
        {
            var isExist =  await _context.Users.AnyAsync(x => x.Id == userId);
            if (!isExist)
                return Result.Failure<IQueryable<ApplicationUser>>(UserErrors.NotFouond);

            return Result.Success( _context.Follows
                .Where(x => x.FollowingId == userId && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => x.Follower)
                .AsNoTracking());
        }


        //public async Task<int> GetFollowersCountAsync(string userId, CancellationToken cancellation = default)
        //{
        //    return await _context.Follows
        //        .CountAsync(x => x.FollowingId == userId && !x.IsDeleted, cancellation);
        //}
        //public async Task<int> GetFollowingCountAsync(string userId, CancellationToken cancellation = default)
        //{
        //    return await _context.Follows
        //        .CountAsync(x => x.FollowerId == userId && !x.IsDeleted, cancellation);
        //}
    }
}
