using Account.Domain.Enum;
using Account.Domain.Result;
using System.Net;
using ILogger = Serilog.ILogger;

namespace Account.api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(httpContext, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            _logger.Error(exception, exception.Message);
            if (exception is ExceptionResult exResult)
            {
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = exResult.HttpStatusCode;

                await httpContext.Response.WriteAsJsonAsync(new BaseResult
                {
                    Message = exResult.Message,
                    ErrorCode = (int)exResult.ErrorCode
                });
                return;
            }

            // Кидаем generic ошибку, но логируем реальное исключение
            _logger.Error($"Unhandled exception type: {exception.GetType().FullName}");

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(new BaseResult
            {
                Message = "Internal server error",
                ErrorCode = (int)ErrorCodes.InternalServerError
            });
        }
    }
}