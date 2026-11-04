using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;

namespace MovieTicketBooking.Middleware;

/// <summary>
/// Custom middleware that intercepts incoming requests to /api/bookings
/// and enforces role-based and booking ownership authorization before
/// requests reach the controller.
/// </summary>
public class BookingAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public BookingAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        var path = context.Request.Path.Value?.Trim('/') ?? string.Empty;
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        // Check if the request targets /api/bookings (case-insensitive)
        if (segments.Length < 2 ||
            !segments[0].Equals("api", StringComparison.OrdinalIgnoreCase) ||
            !segments[1].Equals("bookings", StringComparison.OrdinalIgnoreCase))
        {
            // Not a bookings endpoint; pass through to the next middleware
            await _next(context);
            return;
        }

        var method = context.Request.Method.ToUpperInvariant();

        // 1. GET /api/bookings/reference/{reference} is a public lookup endpoint
        if (segments.Length == 4 && segments[2].Equals("reference", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // 2. All other booking operations require an authenticated user
        if (context.User.Identity?.IsAuthenticated != true)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Authentication is required to perform this booking operation."
            });
            return;
        }

        // 3. Extract the authenticated user's ID and role from JWT claims
        var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var currentUserId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Invalid or missing user ID claim in authentication token."
            });
            return;
        }

        var isAdminOrManager = context.User.IsInRole("Admin") || context.User.IsInRole("Manager");

        // 4. GET /api/bookings (Get all bookings)
        // Admin and Manager can retrieve all bookings. Customers are forbidden.
        if (segments.Length == 2 && method == "GET")
        {
            if (!isAdminOrManager)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Customers are not authorized to retrieve all bookings."
                });
                return;
            }

            await _next(context);
            return;
        }

        // 5. POST /api/bookings (Create booking)
        // Authenticated users are allowed to proceed; CustomerId is derived from JWT in the controller
        if (segments.Length == 2 && method == "POST")
        {
            await _next(context);
            return;
        }

        // 6. GET /api/bookings/{id} (Get booking by ID)
        if (segments.Length == 3 && method == "GET" && int.TryParse(segments[2], out var getBookingId))
        {
            if (isAdminOrManager)
            {
                await _next(context);
                return;
            }

            // Customer access: validate ownership against Booking.CustomerId
            var booking = await dbContext.Bookings
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookingId == getBookingId);

            if (booking == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Booking not found."
                });
                return;
            }

            if (booking.CustomerId != currentUserId)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "You are not authorized to view another customer's booking."
                });
                return;
            }

            await _next(context);
            return;
        }

        // 7. PATCH /api/bookings/{id}/cancel (Cancel booking)
        if (segments.Length == 4 &&
            method == "PATCH" &&
            int.TryParse(segments[2], out var cancelBookingId) &&
            segments[3].Equals("cancel", StringComparison.OrdinalIgnoreCase))
        {
            if (isAdminOrManager)
            {
                await _next(context);
                return;
            }

            // Customer access: validate ownership before allowing cancellation
            var booking = await dbContext.Bookings
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookingId == cancelBookingId);

            if (booking == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Booking not found."
                });
                return;
            }

            if (booking.CustomerId != currentUserId)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "You are not authorized to cancel another customer's booking."
                });
                return;
            }

            await _next(context);
            return;
        }

        // Pass through any other routes to subsequent middleware
        await _next(context);
    }
}

