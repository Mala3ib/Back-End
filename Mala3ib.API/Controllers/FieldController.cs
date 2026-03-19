using System.Security.Claims;
using Mala3ib.BLL.Contracts.Field;

namespace Mala3ib.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
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
        public async Task<IActionResult> GetAll()
        {
            var result = await _fieldService.GetAllAsync();

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellation)
        {
            var result = await _fieldService.GetByIdAsync(id, cancellation);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpGet("get-by-owner-id")]
        public async Task<IActionResult> GetByOwnerId(int ownerId, CancellationToken cancellation)
        {
            var result = await _fieldService.GetByOwnerIdAsync(ownerId, cancellation);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpPost("")]
        public async Task<IActionResult> Add(AddFieldRequestDto request, CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerId = await _fieldOwnerService.GetOwnerIdByUserIdAsync(userId!);

            var result = await _fieldService.AddAsync(request, ownerId.Value, cancellation);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpDelete("")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerId = await _fieldOwnerService.GetOwnerIdByUserIdAsync(userId!);

            var result = await _fieldService.DeleteAsync(id, ownerId.Value, cancellation);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpPut("")]
        public async Task<IActionResult> Update(int id, UpdateFieldRequestDto request, CancellationToken cancellation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerId = await _fieldOwnerService.GetOwnerIdByUserIdAsync(userId!);

            var result = await _fieldService.UpdateAsync(id, request, ownerId.Value, cancellation);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}