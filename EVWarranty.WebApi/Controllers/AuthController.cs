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
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var result = await _authService.LoginWithGoogleAsync(request);
            if (result == null)
                return BadRequest(new { message = "Xác thực Google thất bại hoặc Token không hợp lệ" });

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var success = await _authService.ForgotPasswordAsync(request);
            // Lưu ý: Luôn trả về Ok để tránh kẻ xấu dò tìm email tồn tại trong hệ thống
            return Ok(new { message = "Nếu email tồn tại, một mã OTP đã được gửi đi." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var success = await _authService.ResetPasswordAsync(request);
            if (!success)
                return BadRequest(new { message = "Mã OTP không hợp lệ hoặc đã hết hạn" });

            return Ok(new { message = "Mật khẩu đã được thay đổi thành công" });
        }

    }
}