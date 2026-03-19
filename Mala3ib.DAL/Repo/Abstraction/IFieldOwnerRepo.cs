namespace Mala3ib.DAL.Repo.Abstraction
{
    public interface IFieldOwnerRepo
    {
        Task AddAsync(FieldOwner fieldOwner);
        IQueryable<FieldOwner> Get(string userId);
        Task<Result> DeleteAsync(string userId, CancellationToken cancellation = default);
        Task<Result> UpdateAsync(string userId, FieldOwner request, CancellationToken cancellation = default);
        IQueryable<FieldOwner> GetOwnerByUserId(string userId);
    }
}
