using EVWarranty.Application.DTOs.Auth;

namespace EVWarranty.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
    Task<LoginResponse?> LoginWithGoogleAsync(GoogleLoginRequest request);
}
