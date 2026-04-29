using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EVWarranty.WebApi.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // 1. Log lỗi để admin có thể kiểm tra sau này
        _logger.LogError(exception, "Một lỗi không mong muốn đã xảy ra: {Message}", exception.Message);

        // 2. Tạo cấu trúc trả về chuẩn ProblemDetails (RFC 7807)
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server Error",
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Detail = "Đã có lỗi xảy ra trên hệ thống. Vui lòng liên hệ Admin hoặc thử lại sau."
        };

        // 3. Trả về cho Client
        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Trả về true để báo cho hệ thống rằng lỗi đã được xử lý
        return true;
    }
}
