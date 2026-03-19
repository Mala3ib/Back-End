namespace Mala3ib.DAL.Repo.Abstraction
{
    public interface IFieldRepo
    {
        Task AddAsync(Field field);
        IQueryable<Field> GetAll();
        IQueryable<Field> GetByOwnerId(int fieldOwnerId);
        IQueryable<Field> GetById(int id);
        Task<Result> UpdateAsync(int id, Field request, CancellationToken cancellation = default);
        Task<Result> DeleteAsync(int id, CancellationToken cancellation = default);
    }
}