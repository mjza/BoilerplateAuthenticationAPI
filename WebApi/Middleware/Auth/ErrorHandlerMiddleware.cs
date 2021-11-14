using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using WebApi.Helpers.Auth;

namespace WebApi.Middleware.Auth
{
    // a record for creating error messages
    internal record MessageRecord(string Message);

    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                _logger.LogError(exception, exception.Message);

                response.StatusCode = exception switch
                {
                    AppException => (int)HttpStatusCode.BadRequest,// custom application exception
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,// not found exception
                    _ => (int)HttpStatusCode.InternalServerError,// unhandled exception                        
                };

                var result = JsonSerializer.Serialize(new MessageRecord(exception?.Message));
                await response.WriteAsync(result);
            }
        }
    }
}