using EVWarranty.Application.DTOs.Auth;
using EVWarranty.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EVWarranty.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (result == null) return Unauthorized();
            return Ok(result);
        }

    }
}