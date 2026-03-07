
using Mala3ib.DAL.Database;

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
    }
}
