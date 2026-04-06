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

        public async Task<bool> IsExist(int id, CancellationToken cancellation = default)
        {
            return await _context.FieldSlots
                .AnyAsync(f => f.Id == id && !f.IsDeleted, cancellation);
        }

        public async Task UpdateAsync(int id, FieldSlot request, CancellationToken cancellation = default)
        {
            await _context.FieldSlots
                .Where(f => f.Id == id)
                .ExecuteUpdateAsync(setter =>
                    setter.SetProperty(x => x.StartDate, request.StartDate)
                    .SetProperty(x => x.EndDate, request.EndDate)
                    .SetProperty(x => x.IsBooked, request.IsBooked)
                    .SetProperty(x => x.Price, request.Price)
                    .SetProperty(x => x.MaxPlayers, request.MaxPlayers)
                );
        }

        public async Task DeleteAsync(int id, CancellationToken cancellation = default)
        {
            await _context.FieldSlots
                .Where(f => f.Id == id)
                .ExecuteUpdateAsync(setter =>
                 setter.SetProperty(x => x.IsDeleted, true)
            );
        }
    }
}