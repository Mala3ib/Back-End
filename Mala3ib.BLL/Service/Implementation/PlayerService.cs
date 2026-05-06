
namespace Mala3ib.BLL.Service.Implementation
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepo _playerRepo;
        private readonly IFollowRepo _followRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        public PlayerService(IPlayerRepo playerRepo,
            UserManager<ApplicationUser> userManager,
            IFollowRepo followRepo)
        {
            _playerRepo = playerRepo;
            _userManager = userManager;
            _followRepo = followRepo;
        }

        public async Task<Result<PlayerProfileDto>> GetAsync(string currentUserId, string userId, CancellationToken cancellation = default)
        {
            var player = await _playerRepo.Get(userId)
                .Select(p => new PlayerProfileDto(
                    p.User.Email!,
                    p.User.FirstName,
                    p.User.LastName,
                    p.User.PhoneNumber!,
                    p.DateOfBirth,
                    p.User.Followers.Count(x => !x.IsDeleted),
                    p.User.Following.Count(x => !x.IsDeleted),
                    currentUserId == userId ? false : p.User.Followers.Any(x => x.FollowerId == currentUserId && !x.IsDeleted)
                ))
                .FirstOrDefaultAsync(cancellation);

            if (player is null)
                return Result.Failure<PlayerProfileDto>(PlayerErrors.NotFound);

            return Result.Success(player);
        }

        public async Task<Result> UpdateAsync(string userId, UpdatePlayerRequestDto request, CancellationToken cancellation = default)
        {
            var oldPlayer = _playerRepo.Get(userId).FirstOrDefault();

            if (oldPlayer == null)
                return Result.Failure(PlayerErrors.NotFound);

            var player = new Player
            {
                DateOfBirth = request.DateOfBirth,
                User = new ApplicationUser
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber
                }
            };

            await _playerRepo.UpdateAsync(userId, player, cancellation);
            return Result.Success(player);
        }

        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequestDto request)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

            if (result.Succeeded)
            {
                return Result.Success();
            }

            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, ErrorType.BadRequest));
        }

        public async Task<Result> DeleteAsync(string userId, CancellationToken cancellation = default)
        {
            var oldPlayer = _playerRepo.Get(userId).FirstOrDefault();

            if (oldPlayer == null)
                return Result.Failure(PlayerErrors.NotFound);

            await _playerRepo.DeleteAsync(userId, cancellation);

            return Result.Success();
        }

    }
}
