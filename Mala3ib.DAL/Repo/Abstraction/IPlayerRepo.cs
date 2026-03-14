namespace Mala3ib.DAL.Repo.Abstraction
{
    public interface IPlayerRepo
    {
        Task AddAsync(Player player);
        IQueryable<Player> Get(string userId);
        Task<Result> DeleteAsync(string userId, CancellationToken cancellation = default);
        Task<Result> UpdateAsync(string userId, Player request, CancellationToken cancellation = default);
    }
}
