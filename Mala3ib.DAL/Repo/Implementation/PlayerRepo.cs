namespace Mala3ib.DAL.Repo.Implementation
{
    public class PlayerRepo : IPlayerRepo
    {
        private readonly ApplicationDbContext _context;

        public PlayerRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Player player)
        {
            await _context.Players.AddAsync(player);

            await _context.SaveChangesAsync();
        }      

        public  IQueryable<Player> Get(string userId)
        {
            var player = _context.Players
                .Where(x => x.UserId == userId)
                .AsNoTracking();

            return player;
        }

        public async Task<Result> UpdateAsync(string userId, Player request, CancellationToken cancellation = default)
        {
            var isExist = await _context.Players
                .AnyAsync(p => p.UserId == userId && !p.IsDeleted, cancellation);

            if (!isExist)
                return Result.Failure(PlayerErrors.NotFound);

            await _context.Players
                .Where(p => p.UserId == userId)
                .ExecuteUpdateAsync(setter =>
                    setter.SetProperty(x => x.DateOfBirth, request.DateOfBirth)
                );

            await _context.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(setter =>
                     setter.SetProperty(x => x.FirstName, request.User.FirstName)
                    .SetProperty(x => x.LastName, request.User.LastName)
                    .SetProperty(x => x.PhoneNumber, request.User.PhoneNumber)
                );

            await _context.SaveChangesAsync(cancellation);
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(string userId, CancellationToken cancellation = default)
        {
            var isExist = await _context.Players
                .AnyAsync(p => p.UserId == userId && !p.IsDeleted, cancellation);
            
            if(!isExist)
                return Result.Failure(PlayerErrors.NotFound);

            var player = _context.Players
                .Include(x => x.User)
                .FirstOrDefault(x => x.UserId == userId);

            player!.IsDeleted = true;
            player!.User.IsDeleted = true;

            await _context.SaveChangesAsync(cancellation);
            return Result.Success();
        }
    }
}
