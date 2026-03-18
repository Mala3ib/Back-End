
namespace Mala3ib.BLL.Service.Implementation
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepo _playerRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        public PlayerService(IPlayerRepo playerRepo, UserManager<ApplicationUser> userManager)
        {
            _playerRepo = playerRepo;
            _userManager = userManager;
        }

        public async Task<Result<PlayerProfileDto>> GetAsync(string userId, CancellationToken cancellation = default)
        {
            var player = await _playerRepo.Get(userId)
                .Select(p => new PlayerProfileDto(
                    p.User.Email!,
                    p.User.FirstName,
                    p.User.LastName,
                    p.User.PhoneNumber!,
                    p.DateOfBirth
                ))
                .FirstOrDefaultAsync(cancellation);

            if (player is null)
                return Result.Failure<PlayerProfileDto>(PlayerErrors.NotFound);

            return Result.Success(player);
        }

        public async Task<Result> UpdateAsync(string userId, UpdatePlayerRequestDto request, CancellationToken cancellation = default)
        {
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

            return await _playerRepo.UpdateAsync(userId, player, cancellation);
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
            return await _playerRepo.DeleteAsync(userId, cancellation);
        }


    }
}
