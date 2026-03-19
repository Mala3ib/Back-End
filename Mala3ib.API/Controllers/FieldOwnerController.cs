namespace Mala3ib.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class FieldOwnerController : ControllerBase
    {
        private readonly IFieldOwnerService _fieldOwnerService;
        public FieldOwnerController(IFieldOwnerService fieldOwnerService)
        {
            _fieldOwnerService = fieldOwnerService;
        }
        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _fieldOwnerService.GetAsync(userId!);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var uesrId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _fieldOwnerService.ChangePasswordAsync(uesrId!, request);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpDelete("")]
        public async Task<IActionResult> Delete(CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _fieldOwnerService.DeleteAsync(userId!);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpPut("")]
        public async Task<IActionResult> Update(UpdateFieldOwnerRequestDto request, CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _fieldOwnerService.UpdateAsync(userId!, request, cancellation);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}