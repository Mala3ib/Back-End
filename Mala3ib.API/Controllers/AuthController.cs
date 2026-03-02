using Mala3ib.BLL.Contracts.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mala3ib.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellation)
        {
            var result = await _authService.GetTokenAsync(request.Email, request.Password, cancellation);

            return result!.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var refreshResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

            return refreshResult!.IsSuccess ? Ok(refreshResult.Value) : refreshResult.ToProblem();  
        }
    }
}
