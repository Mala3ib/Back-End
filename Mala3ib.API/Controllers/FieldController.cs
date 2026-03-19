using Mala3ib.DAL.Abstraction.Const;

namespace Mala3ib.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    public class FieldController : ControllerBase
    {
        private readonly IFieldService _fieldService;
        private readonly IFieldOwnerService _fieldOwnerService;

        public FieldController(IFieldService fieldService, IFieldOwnerService fieldOwnerService)
        {
            _fieldService = fieldService;
            _fieldOwnerService = fieldOwnerService;
        }

        [HttpGet("get-all")]
        [Authorize(Roles = $"{DefaultRoles.FieldOwner},{DefaultRoles.Player}")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _fieldService.GetAllAsync();

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellation)
        {
            var result = await _fieldService.GetByIdAsync(id, cancellation);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpGet("get-by-owner-id/{ownerId}")]
        public async Task<IActionResult> GetByOwnerId([FromRoute] int ownerId, CancellationToken cancellation)
        {
            var result = await _fieldService.GetByOwnerIdAsync(ownerId, cancellation);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody] AddFieldRequestDto request, CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerId = await _fieldOwnerService.GetOwnerIdByUserIdAsync(userId!);

            var result = await _fieldService.AddAsync(request, ownerId.Value, cancellation);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerId = await _fieldOwnerService.GetOwnerIdByUserIdAsync(userId!);

            var result = await _fieldService.DeleteAsync(id, ownerId.Value, cancellation);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, UpdateFieldRequestDto request, CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerId = await _fieldOwnerService.GetOwnerIdByUserIdAsync(userId!);

            var result = await _fieldService.UpdateAsync(id, request, ownerId.Value, cancellation);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}