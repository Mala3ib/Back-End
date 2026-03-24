namespace Mala3ib.DAL.Repo.Implementation
{
    public class FieldSlotRepo : IFieldSlotRepo
    {
        private readonly ApplicationDbContext _context;
        public FieldSlotRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(FieldSlot fieldSlot)
        {
            await _context.FieldSlots.AddAsync(fieldSlot);

            await _context.SaveChangesAsync();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellation = default)
        {
            var isExist = await _context.FieldSlots.AnyAsync(p => p.Id == id && !p.IsDeleted, cancellation);

            if (!isExist)
                return Result.Failure(FieldSlotErrors.NotFound);

            var fieldSlot = _context.FieldSlots
                .FirstOrDefault(x => x.Id == id);

            fieldSlot!.IsDeleted = true;

            await _context.SaveChangesAsync(cancellation);
            return Result.Success();
        }

        public IQueryable<FieldSlot> GetAvailableSlots(int fieldId, DateTime day)
        {
            var startOfDay = day.Date;
            var endOfDay = startOfDay.AddDays(1);

            var fieldSlots = _context.FieldSlots
                .Where(x =>
                    x.FieldId == fieldId &&
                    !x.IsDeleted &&
                    !x.IsBooked &&
                    x.StartDate >= startOfDay &&
                    x.StartDate < endOfDay
                )
                .AsNoTracking();

            return fieldSlots;
        }

        public IQueryable<FieldSlot> GetByFieldId(int fieldId)
        {
            var fieldSlot = _context.FieldSlots
                .Where(x => x.FieldId == fieldId && !x.IsDeleted)
                .AsNoTracking();

            return fieldSlot;
        }

        public IQueryable<FieldSlot> GetById(int id)
        {
            var fieldSlot = _context.FieldSlots
                .Where(x => x.Id == id && !x.IsDeleted)
                .AsNoTracking();

            return fieldSlot;
        }

        public async Task<bool> IsSlotAvailableAsync(int fieldId, DateTime start, DateTime end, int? excludeSlotId = null, CancellationToken cancellation = default)
        {
            if (start >= end)
                return false;

            var exists = await _context.FieldSlots.AnyAsync(s =>
                s.FieldId == fieldId &&
                !s.IsDeleted &&
                (excludeSlotId == null || s.Id != excludeSlotId) &&
                start < s.EndDate &&
                end > s.StartDate
            );

            return !exists;
        }
        public async Task<Result> UpdateAsync(int id, FieldSlot request, CancellationToken cancellation = default)
        {
            var isExist = await _context.FieldSlots
                .AnyAsync(f => f.Id == id && !f.IsDeleted, cancellation);

            if (!isExist)
                return Result.Failure(FieldSlotErrors.NotFound);

            await _context.FieldSlots
                .Where(f => f.Id == id)
                .ExecuteUpdateAsync(setter =>
                    setter.SetProperty(x => x.StartDate, request.StartDate)
                    .SetProperty(x => x.EndDate, request.EndDate)
                    .SetProperty(x => x.IsBooked, request.IsBooked)
                );

            await _context.SaveChangesAsync(cancellation);
            return Result.Success();
        }
    }
}