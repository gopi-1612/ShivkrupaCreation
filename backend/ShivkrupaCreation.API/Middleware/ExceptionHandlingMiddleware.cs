using FluentValidation;
using ShivkrupaCreation.API.DTOs.Common;
using System.Net;
using System.Text.Json;

namespace ShivkrupaCreation.API.Middleware
{
    /// <summary>
    /// Catches every unhandled exception in the pipeline and converts it into
    /// a consistent ApiResponse JSON error, so the Angular frontend never has
    /// to special-case raw 500s or ASP.NET's default problem-details shape.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                ApiException apiEx => ApiResponse<object>.FailureResponse(apiEx.Message, apiEx.Errors),

                ValidationException validationEx => ApiResponse<object>.FailureResponse(
                    "Validation failed.",
                    validationEx.Errors.Select(e => e.ErrorMessage).ToList()),

                UnauthorizedAccessException => ApiResponse<object>.FailureResponse("You are not authorized to perform this action."),

                _ => ApiResponse<object>.FailureResponse(
                    _env.IsDevelopment() ? exception.Message : "An unexpected error occurred. Please try again later.")
            };

            context.Response.StatusCode = exception switch
            {
                ApiException apiEx => apiEx.StatusCode,
                ValidationException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.InternalServerError
            };

            if (context.Response.StatusCode == (int)HttpStatusCode.InternalServerError)
            {
                _logger.LogError(exception, "Unhandled exception on {Path}", context.Request.Path);
            }

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
