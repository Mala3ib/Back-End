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

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellation = default)
        {
            var isExist = await _context.Fields.AnyAsync(p => p.Id == id && !p.IsDeleted, cancellation);

            if (!isExist)
                return Result.Failure(FieldErrors.NotFound);

            var field = _context.Fields
                .FirstOrDefault(x => x.Id == id);

            field!.IsDeleted = true;

            await _context.SaveChangesAsync(cancellation);
            return Result.Success();
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

        public async Task<Result> UpdateAsync(int id, Field request, CancellationToken cancellation = default)
        {
            var isExist = await _context.Fields
                .AnyAsync(f => f.Id == id && !f.IsDeleted, cancellation);

            if (!isExist)
                return Result.Failure(FieldErrors.NotFound);

            await _context.Fields
                .Where(f => f.Id == id)
                .ExecuteUpdateAsync(setter =>
                    setter.SetProperty(x => x.Name, request.Name)
                    .SetProperty(x => x.Location, request.Location)
                    .SetProperty(x => x.PricePerHour, request.PricePerHour)
                );

            await _context.SaveChangesAsync(cancellation);
            return Result.Success();
        }
    }
}