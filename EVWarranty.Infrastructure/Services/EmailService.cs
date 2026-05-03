using EVWarranty.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace EVWarranty.Infrastructure.Services;

public class EmailService : IEmailService
{
    public async Task SendOtpEmailAsync(string toEmail, string otpCode)
    {
        // Mentor: Đây là bản giả lập, mã OTP sẽ in ra cửa sổ Terminal của bạn
        Console.WriteLine("================================================");
        Console.WriteLine($"[EMAIL SERVICE] Đang gửi mã OTP: {otpCode}");
        Console.WriteLine($"[EMAIL SERVICE] Tới địa chỉ: {toEmail}");
        Console.WriteLine("================================================");
        
        await Task.CompletedTask;
    }
}
