using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SalesInventoryManagement.Application.Exceptions;

namespace SalesInventoryManagement.API.Middleware
{
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
            _logger.LogError(exception, "حصل خطأ غير متوقع: {Message}", exception.Message);

            var (statusCode, title) = exception switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "المورد غير موجود"),
                BusinessRuleException => (StatusCodes.Status400BadRequest, "خطأ في قاعدة العمل"),
                UnauthorizedException => (StatusCodes.Status401Unauthorized, "بيانات الدخول غير صحيحة"), // 👈 جديد
                _ => (StatusCodes.Status500InternalServerError, "حصل خطأ غير متوقع في السيرفر")
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = exception.Message
            };

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true; 
        }
    }
}