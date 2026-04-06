namespace Mala3ib.DAL.Repo.Implementation
{
    public class FieldRepo : IFieldRepo
    {
        private readonly ApplicationDbContext _context;

        public FieldRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Field field)
        {
            await _context.Fields.AddAsync(field);

            await _context.SaveChangesAsync();
        }

        public IQueryable<Field> GetAll()
        {
            var fields = _context.Fields.Where(f => !f.IsDeleted)
                .AsNoTracking();

            return fields;
        }

        public IQueryable<Field> GetById(int id)
        {
            var field = _context.Fields
                .Where(x => x.Id == id && !x.IsDeleted)
                .AsNoTracking();

            return field;
        }

        public IQueryable<Field> GetByOwnerId(int fieldOwnerId)
        {
            var field = _context.Fields
                .Where(x => x.FieldOwnerId == fieldOwnerId && !x.IsDeleted)
                .AsNoTracking();

            return field;
        }

        public async Task<decimal> GetPrice(int id, CancellationToken cancellation = default)
        {
            return await _context.Fields
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => x.PricePerHour)
                .FirstOrDefaultAsync(cancellation);
        }

        public async Task<bool> IsExistAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Fields
                .AnyAsync(x => x.Id == id  && !x.IsDeleted, cancellationToken);
        }

        public async Task UpdateAsync(int id, Field request, CancellationToken cancellation = default)
        {
            await _context.Fields
                .Where(f => f.Id == id)
                .ExecuteUpdateAsync(setter =>
                    setter.SetProperty(x => x.Name, request.Name)
                    .SetProperty(x => x.Location, request.Location)
                    .SetProperty(x => x.PricePerHour, request.PricePerHour)
                );
        }

        public async Task DeleteAsync(int id, CancellationToken cancellation = default)
        {
            var affectedRows = await _context.Fields
                .Where(x => x.Id == id && !x.IsDeleted)
                .ExecuteUpdateAsync(setter =>
                setter.SetProperty(x => x.IsDeleted, true)
                );
        }
    }
}