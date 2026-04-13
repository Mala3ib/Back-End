namespace Mala3ib.DAL.Repo.Abstraction
{
    public interface IFieldOwnerRepo
    {
        Task AddAsync(FieldOwner fieldOwner);
        IQueryable<FieldOwner> Get(string userId);
        Task DeleteAsync(string userId, CancellationToken cancellation = default);
        Task UpdateAsync(string userId, FieldOwner request, CancellationToken cancellation = default);
        IQueryable<FieldOwner> GetOwnerByUserId(string userId);
        Task<bool> IsExistAsync(string userId, CancellationToken cancellation = default);
    }
}
