using EVWarranty.Application.DTOs.Auth;

namespace EVWarranty.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}
