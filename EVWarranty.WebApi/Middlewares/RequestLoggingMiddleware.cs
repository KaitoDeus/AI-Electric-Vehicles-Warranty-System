using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System;

namespace EVWarranty.WebApi.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Lấy thông tin cơ bản
            var method = context.Request.Method;
            var path = context.Request.Path;
            var queryString = context.Request.QueryString;

            // Lấy màu sắc 
            string methodColor = GetMethodColor(method);
            string reset = "\x1b[0m";

            // In ra đúng 1 dòng: [GET] /api/vehicles
            Console.WriteLine($"[{methodColor}{method}{reset}] {path}{queryString}");

            // Đi tiếp
            await _next(context);
        }


        private string GetMethodColor(string method)
        {
            return method.ToUpper() switch
            {
                "GET" => "\x1b[32m",    // Green
                "POST" => "\x1b[33m",   // Yellow
                "PUT" => "\x1b[34m",    // Blue
                "DELETE" => "\x1b[31m", // Red
                _ => "\x1b[37m"         // White
            };
        }

        private string GetStatusColor(int statusCode)
        {
            if (statusCode >= 200 && statusCode < 300) return "\x1b[32m"; // Green
            if (statusCode >= 400) return "\x1b[31m";                     // Red
            return "\x1b[33m";                                            // Yellow
        }
    }
}
