
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
    // Admin/Manager: all bookings
    // Customer: own bookings
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customerId = IsAdminOrManager()
            ? null
            : GetCurrentUserId();

        if (!IsAdminOrManager() && customerId == null)
            return Unauthorized();

        var bookings = await _service.GetAllAsync(customerId);

        return Ok(bookings);
    }

    // GET: api/bookings/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var customerId = IsAdminOrManager()
            ? null
            : GetCurrentUserId();

        if (!IsAdminOrManager() && customerId == null)
            return Unauthorized();

        var booking = await _service.GetByIdAsync(id, customerId);

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
    // Only authenticated customers can create bookings
    [Authorize(Roles = "Customer")]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBookingRequest request)
    {
        var customerId = GetCurrentUserId();

        if (customerId == null)
            return Unauthorized();

        try
        {
            var booking = await _service.CreateAsync(
                request,
                customerId.Value);

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
    [HttpPatch("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var customerId = IsAdminOrManager()
            ? null
            : GetCurrentUserId();

        if (!IsAdminOrManager() && customerId == null)
            return Unauthorized();

        try
        {
            var result = await _service.CancelAsync(
                id,
                customerId);

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

    private int? GetCurrentUserId()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        return int.TryParse(userId, out var id)
            ? id
            : null;
    }

    private bool IsAdminOrManager()
    {
        return User.IsInRole("Admin") ||
               User.IsInRole("Manager");
    }
}
