using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MovieTicketBooking.Middleware;

/// <summary>
/// Diagnostic middleware that traces the start of authorization evaluation
/// and logs the final HTTP status code after downstream authorization and controller processing.
/// </summary>
public class AuthorizationDiagnosticsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthorizationDiagnosticsMiddleware> _logger;

    public AuthorizationDiagnosticsMiddleware(
        RequestDelegate next,
        ILogger<AuthorizationDiagnosticsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("\n========== AUTHORIZATION PROCESS ==========");
        _logger.LogInformation(
            "Evaluating Authorization for: {Method} {Path}",
            context.Request.Method,
            context.Request.Path
        );

        await _next(context);

        _logger.LogInformation(
            "Final Response Status: {StatusCode} for {Method} {Path}",
            context.Response.StatusCode,
            context.Request.Method,
            context.Request.Path
        );
        _logger.LogInformation("===========================================\n");
    }
}
