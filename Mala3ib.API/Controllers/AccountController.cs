namespace Mala3ib.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly IFieldOwnerService _fieldOwnerService;
        public AccountController(IPlayerService playerService, IFieldOwnerService fieldOwnerService)
        {
            _playerService = playerService;
            _fieldOwnerService = fieldOwnerService;
        }

        [Authorize]
        [HttpGet("player/{userId}")]
        public async Task<IActionResult> GetPlayer([FromRoute] string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _playerService.GetAsync(currentUserId!, userId);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [Authorize(Roles = DefaultRoles.FieldOwner)]
        [HttpGet("field-owner")]
        public async Task<IActionResult> GetOwner()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _fieldOwnerService.GetAsync(userId!);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [Authorize(Roles = DefaultRoles.Player)]
        [HttpPut("player/change-password")]
        public async Task<IActionResult> ChangePlayerPassword([FromBody] ChangePasswordRequestDto request)
        {
            var uesrId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _playerService.ChangePasswordAsync(uesrId!, request);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [Authorize(Roles = DefaultRoles.FieldOwner)]
        [HttpPut("field-owner/change-password")]
        public async Task<IActionResult> ChangeOwnerPassword([FromBody] ChangePasswordRequestDto request)
        {
            var uesrId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _fieldOwnerService.ChangePasswordAsync(uesrId!, request);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [Authorize(Roles = DefaultRoles.Player)]
        [HttpDelete("player")]
        public async Task<IActionResult> DeletePlayer(CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _playerService.DeleteAsync(userId!);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [Authorize(Roles = DefaultRoles.FieldOwner)]
        [HttpDelete("field-owner")]
        public async Task<IActionResult> DeleteOwner(CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _fieldOwnerService.DeleteAsync(userId!);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [Authorize(Roles = DefaultRoles.Player)]
        [HttpPut("player")]
        public async Task<IActionResult> UpdatePlayer(UpdatePlayerRequestDto request, CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _playerService.UpdateAsync(userId!, request, cancellation);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [Authorize(Roles = DefaultRoles.FieldOwner)]
        [HttpPut("field-owner")]
        public async Task<IActionResult> UpdateOwner(UpdateFieldOwnerRequestDto request, CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _fieldOwnerService.UpdateAsync(userId!, request, cancellation);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
