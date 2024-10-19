using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using PizzaCompany.DTOs;
using System.Net;
using System.Text.Json;

namespace PizzaCompany.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);  // Call the next middleware in the pipeline
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Set default response code and message
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var response = new { message = "Internal Server Error", details = exception.Message };

            // Customize for specific exception types
            if (exception is ArgumentException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new { message = "Bad Request", details = exception.Message };
            }

            var result = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(result);
        }
    }
}
