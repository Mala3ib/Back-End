namespace Mala3ib.DAL.Repo.Abstraction
{
    public interface IFieldSlotRepo
    {
        IQueryable<FieldSlot> GetByFieldId(int fieldId);
        IQueryable<FieldSlot> GetAvailableSlots(int fieldId, DateTime day);
        IQueryable<FieldSlot> GetById(int id);
        Task<bool> IsSlotAvailableAsync(int fieldId, DateTime start, DateTime end, int? excludeSlotId = null, CancellationToken cancellation = default);
        Task AddAsync(FieldSlot fieldSlot);
        Task<bool> IsExist(int id, CancellationToken cancellation = default);
        Task UpdateAsync(int id, FieldSlot request, CancellationToken cancellation = default);
        Task DeleteAsync(int id, CancellationToken cancellation = default);
    }
}