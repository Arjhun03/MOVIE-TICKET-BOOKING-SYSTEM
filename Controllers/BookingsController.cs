using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _service;

    public BookingsController(IBookingService service)
    {
        _service = service;
    }

    // GET: api/bookings
    // Access restricted to Admin and Manager (enforced by BookingAuthorizationMiddleware)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var bookings = await _service.GetAllAsync(null);

        return Ok(bookings);
    }

    // GET: api/bookings/5
    // Ownership authorization is enforced by BookingAuthorizationMiddleware before reaching this action
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var booking = await _service.GetByIdAsync(id, null);

        if (booking == null)
        {
            return NotFound(new
            {
                message = "Booking not found."
            });
        }

        return Ok(booking);
    }

    // GET: api/bookings/reference/BK123
    // Public endpoint for reference lookup
    [AllowAnonymous]
    [HttpGet("reference/{bookingReference}")]
    public async Task<IActionResult> GetByReference(
        string bookingReference)
    {
        var booking = await _service.GetByReferenceAsync(
            bookingReference);

        if (booking == null)
        {
            return NotFound(new
            {
                message = "Booking not found."
            });
        }

        return Ok(booking);
    }

    // POST: api/bookings
    // CustomerId is derived securely from the authenticated JWT token claims
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBookingRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userId, out var customerId))
        {
            return Unauthorized();
        }

        try
        {
            var booking = await _service.CreateAsync(
                request,
                customerId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = booking.BookingId },
                booking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    // PATCH: api/bookings/5/cancel
    // Ownership authorization is enforced by BookingAuthorizationMiddleware before reaching this action
    [HttpPatch("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            var result = await _service.CancelAsync(id, null);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Booking not found."
                });
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }
}
