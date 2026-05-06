namespace Mala3ib.DAL.Repo.Abstraction
{
    public interface IFieldRepo
    {
        Task AddAsync(Field field);
        IQueryable<Field> GetAll();
        IQueryable<Field> GetByOwnerId(int fieldOwnerId);
        IQueryable<Field> GetById(int id);
        Task<decimal> GetPrice(int id, CancellationToken cancellation = default);
        Task UpdateAsync(int id, Field request, CancellationToken cancellation = default);
        Task DeleteAsync(int id, CancellationToken cancellation = default);
        Task<bool> IsExistAsync(int id, CancellationToken cancellationToken = default);
    }
}