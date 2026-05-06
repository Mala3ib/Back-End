using Mala3ib.DAL.Enums;

namespace Mala3ib.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = DefaultRoles.FieldOwner)]
    public class FieldOwnerController : ControllerBase
    {
        private readonly IFieldOwnerPortalService _fieldOwnerPortalService;

        public FieldOwnerController(IFieldOwnerPortalService fieldOwnerPortalService)
        {
            _fieldOwnerPortalService = fieldOwnerPortalService;
        }

        [HttpGet("fields")]
        public async Task<IActionResult> GetMyFields([FromQuery] RequestFilter filter, CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _fieldOwnerPortalService.GetMyFieldsAsync(userId!, filter, cancellation);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> GetMyBookings([FromQuery] RequestFilter filter, CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _fieldOwnerPortalService.GetMyBookingsAsync(userId!, filter, cancellation);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("invitations")]
        public async Task<IActionResult> GetMyInvitations([FromQuery] RequestFilter filter, [FromQuery] InvitationStatus? status, CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _fieldOwnerPortalService.GetMyInvitationsAsync(userId!, filter, status, cancellation);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
    }
}
