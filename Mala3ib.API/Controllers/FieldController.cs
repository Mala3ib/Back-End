using Mala3ib.DAL.Abstraction.Const;

namespace Mala3ib.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FieldController : ControllerBase
    {
        private readonly IFieldService _fieldService;
        public FieldController(IFieldService fieldService, IFieldOwnerService fieldOwnerService)
        {
            _fieldService = fieldService;
        }

        [HttpGet("get-all")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var result = await _fieldService.GetAllAsync();

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [Authorize]
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellation)
        {
            var result = await _fieldService.GetByIdAsync(id, cancellation);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [Authorize]
        [HttpGet("get-by-owner-id/{ownerId}")]
        public async Task<IActionResult> GetByOwnerId([FromRoute] int ownerId, CancellationToken cancellation)
        {
            var result = await _fieldService.GetByOwnerIdAsync(ownerId, cancellation);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [Authorize(Roles = DefaultRoles.FieldOwner)]
        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody] AddFieldRequestDto request, CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _fieldService.AddAsync(request, userId!, cancellation);

            return result.IsSuccess ? CreatedAtAction(nameof(GetById), new {result.Value.Id}, result.Value) : result.ToProblem();
        }

        [Authorize(Roles = DefaultRoles.FieldOwner)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _fieldService.DeleteAsync(id, userId!, cancellation);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [Authorize(Roles = DefaultRoles.FieldOwner)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, UpdateFieldRequestDto request, CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _fieldService.UpdateAsync(id, request, userId!, cancellation);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}