using System.Net;
using System.Text.Json;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Domain.Exceptions;
using ValidationException = ProductsAPI.Domain.Exceptions.ValidationException;

namespace ProductsAPI.API.Middleware;

/// <summary>
/// Global exception handling middleware — catches all unhandled exceptions and returns
/// a consistent JSON error response. Prevents stack traces from leaking to clients.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, response) = exception switch
        {
            NotFoundException nfe => (
                HttpStatusCode.NotFound,
                ApiResponse<object>.Fail(nfe.Message)),

            ValidationException ve => (
                HttpStatusCode.BadRequest,
                ApiResponse<object>.Fail("Validation failed.", ve.Errors)),

            BusinessRuleException bre => (
                HttpStatusCode.BadRequest,
                ApiResponse<object>.Fail(bre.Message)),

            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                ApiResponse<object>.Fail("Unauthorized.")),

            _ => (
                HttpStatusCode.InternalServerError,
                ApiResponse<object>.Fail("An unexpected error occurred. Please try again later."))
        };

        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
    }
}
