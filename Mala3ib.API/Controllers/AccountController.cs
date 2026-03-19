using Mala3ib.DAL.Abstraction.Const;

namespace Mala3ib.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        public AccountController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get([FromRoute] string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);           
            var result = await _playerService.GetAsync(currentUserId!, userId);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var uesrId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _playerService.ChangePasswordAsync(uesrId!, request);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpDelete("")]
        public async Task<IActionResult> Delete(CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _playerService.DeleteAsync(userId!);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpPut("")]
        public async Task<IActionResult> Update(UpdatePlayerRequestDto request, CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _playerService.UpdateAsync(userId!, request, cancellation);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
