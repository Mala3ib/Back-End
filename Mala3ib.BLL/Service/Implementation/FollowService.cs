using Mala3ib.BLL.Contracts.Follow;
using Mapster;

namespace Mala3ib.BLL.Service.Implementation
{
    public class FollowService : IFollowService
    {
        private readonly IFollowRepo _followRepo;

        public FollowService(IFollowRepo followRepo)
        {
            _followRepo = followRepo;
        }

        public async Task<Result> FollowAsync(string currentUserId, FollowRequestDto request, CancellationToken cancellation = default)
        {
            if (currentUserId == request.TargetUserId)
                return Result.Failure(FollowErrors.CannotFollowYourself);

            return await _followRepo.FollowAsync(currentUserId, request.TargetUserId, cancellation);
        }

        public async Task<Result> UnFollowAsync(string currentUserId, FollowRequestDto request, CancellationToken cancellation = default)
        {
            if (currentUserId == request.TargetUserId)
                return Result.Failure(FollowErrors.CannotFollowYourself);

            return await _followRepo.UnFollowAsync(currentUserId, request.TargetUserId, cancellation);
        }

        public async Task<Result<List<FollowUserDto>>> GetFollowingAsync(string userId, CancellationToken cancellation = default)
        {
            var followingResult = await _followRepo.GetFollowing(userId);

            if (followingResult.IsFailure)
                return Result.Failure<List<FollowUserDto>>(followingResult.Error);

            var following = await followingResult.Value
                .Select(x => new FollowUserDto
                (
                    x.Id,
                    $"{x.FirstName} {x.LastName}",
                    x.Image
                ))
                .ToListAsync(cancellation);

            return Result.Success(following);
        }

        public async Task<Result<List<FollowUserDto>>> GetFollowersAsync(string userId, CancellationToken cancellation = default)
        {
            var followersResult = await _followRepo.GetFollowers(userId);

            if (followersResult.IsFailure)
                return Result.Failure<List<FollowUserDto>>(followersResult.Error);

            var followers = await followersResult.Value
                .Select(x => new FollowUserDto
                (
                    x.Id,
                    $"{x.FirstName} {x.LastName}",
                    x.Image
                ))
                .ToListAsync(cancellation);

            return Result.Success(followers);
        }

        //public async Task<Result<int>> GetFollowersCountAsync(string userId, CancellationToken cancellation = default)
        //{
        //    return await _followRepo.GetFollowersCountAsync(userId);
        //}
        //public async Task<Result<int>> GetFollowingCountAsync(string userId, CancellationToken cancellation = default)
        //{
        //    return await _followRepo.GetFollowingCountAsync(userId);
        //}
    }
}
