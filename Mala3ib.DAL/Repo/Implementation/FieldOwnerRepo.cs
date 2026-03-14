
using Mala3ib.DAL.Database;

namespace Mala3ib.DAL.Repo.Implementation
{
    public class FieldOwnerRepo : IFieldOwnerRepo
    {
        private readonly ApplicationDbContext _context;

        public FieldOwnerRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(FieldOwner fieldOwner)
        {
            await _context.FieldOwners.AddAsync(fieldOwner);
            await _context.SaveChangesAsync();
        }
    }
}
