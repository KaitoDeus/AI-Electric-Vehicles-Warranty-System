using EVWarranty.Application.DTOs.Auth;
using EVWarranty.Application.Interfaces;
using EVWarranty.Domain.Entities;
using EVWarranty.Domain.Entities.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;

namespace EVWarranty.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _emailService = emailService;
    }
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        // 1. Tìm danh sách user (kết quả trả về là IEnumerable)
        var users = await _unitOfWork.Users.FindAsync(u => u.Username == request.Username);
        var user = users.FirstOrDefault(); // Lấy thằng đầu tiên hoặc null

        // 2. Kiểm tra tồn tại và mật khẩu
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return null; // Không throw lỗi ở đây để tránh crash 500
        }

        // 3. Lấy thông tin Role thực tế
        string roleName = "Guest";
        if (user.RoleId.HasValue)
        {
            var role = await _unitOfWork.Repository<Role>().GetByIdAsync(user.RoleId.Value);
            roleName = role?.RoleName ?? "Guest";
        }

        // 4. Khởi tạo Response
        return new LoginResponse
        {
            Username = user.Username,
            FullName = user.FullName ?? "",
            Token = GenerateJwtToken(user, roleName),
            Role = roleName
        };
    }
    private string GenerateJwtToken(User user, string roleName)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Role, roleName)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    public async Task<LoginResponse?> LoginWithGoogleAsync(GoogleLoginRequest request)
    {
        try
        {
            // 1. Verify token với Google
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
            // 2. Tìm User theo Email từ thông tin Google trả về
            var users = await _unitOfWork.Users.FindAsync(u => u.Email == payload.Email);
            var user = users.FirstOrDefault();

            // 3. Nếu chưa có -> Tạo mới User
            if (user == null)
            {
                user = new User
                {
                    Username = payload.Email,
                    FullName = payload.Name,
                    Email = payload.Email,
                    RoleId = 5,
                    IsActive = true
                };
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.CompleteAsync();
            }

            // 4. Tạo và trả về JWT Token giống như hàm Login thường
            string roleName = "Customer"; // Vì đăng nhập Google thường là khách
            if (user.RoleId.HasValue)
            {
                var role = await _unitOfWork.Repository<Role>().GetByIdAsync(user.RoleId.Value);
                roleName = role?.RoleName ?? "Customer";
            }

            return new LoginResponse
            {
                Username = user.Username,
                FullName = user.FullName ?? "",
                Token = GenerateJwtToken(user, roleName),
                Role = roleName
            };
        }
        catch
        {
            return null;
        }
    }
    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        // 1. Tìm user theo email
        var users = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email);
        var user = users.FirstOrDefault();
        if (user == null) return false;

        // 2. Tạo mã OTP (6 số ngẫu nhiên)
        var otpCode = Random.Shared.Next(100000, 999999).ToString();

        // 3. Lưu OTP vào bảng OtpTokens
        var otpToken = new OtpToken
        {
            UserId = user.UserId,
            OtpCode = otpCode,
            ExpiryTime = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<OtpToken>().AddAsync(otpToken);
        await _unitOfWork.CompleteAsync();

        // 4. Gửi email chứa OTP
        await _emailService.SendOtpEmailAsync(user.Email!, otpCode);

        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
    {
        // 1. Tìm user theo email
        var users = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email);
        var user = users.FirstOrDefault();
        if (user == null) return false;

        // 2. Kiểm tra OTP hợp lệ (đúng mã, chưa dùng, chưa hết hạn)
        var otpTokens = await _unitOfWork.Repository<OtpToken>().FindAsync(t => 
            t.UserId == user.UserId && 
            t.OtpCode == request.OtpCode && 
            (t.IsUsed == false || t.IsUsed == null) && 
            t.ExpiryTime > DateTime.UtcNow);
        
        var otpToken = otpTokens.OrderByDescending(t => t.CreatedAt).FirstOrDefault();
        if (otpToken == null) return false;

        // 3. Hash mật khẩu mới và cập nhật
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        
        // 4. Đánh dấu OTP đã sử dụng
        otpToken.IsUsed = true;

        await _unitOfWork.CompleteAsync();
        return true;
    }

}
