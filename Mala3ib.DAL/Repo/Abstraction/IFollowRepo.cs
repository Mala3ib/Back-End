namespace Mala3ib.DAL.Repo.Abstraction
{
    public interface IFollowRepo
    {
        Task<Result> FollowAsync(string myPlayerId, string targetPlayerId, CancellationToken cancellation = default);
        Task<Result> UnFollowAsync(string myUserId, string targetUserId, CancellationToken cancellation = default);
        Task<Result<IQueryable<ApplicationUser>>> GetFollowing(string userId);
        Task<Result<IQueryable<ApplicationUser>>> GetFollowers(string userId);
        //Task<int> GetFollowersCountAsync(string userId, CancellationToken cancellation = default);
        //Task<int> GetFollowingCountAsync(string userId, CancellationToken cancellation = default);
    }
}
