using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;

    public BookingService(IBookingRepository repository)
    {
        _repository = repository;
    }

    // Admin/Manager: all bookings
    // Customer: only their bookings
    public async Task<List<BookingResponse>> GetAllAsync(
        int? customerId)
    {
        var bookings = await _repository.GetAllAsync(customerId);

        return bookings.Select(MapToResponse).ToList();
    }

    // Admin/Manager: any booking
    // Customer: only their booking
    public async Task<BookingResponse?> GetByIdAsync(
        int bookingId,
        int? customerId)
    {
        var booking = await _repository.GetByIdAsync(
            bookingId,
            customerId);

        return booking == null ? null : MapToResponse(booking);
    }

    public async Task<BookingResponse?> GetByReferenceAsync(
        string bookingReference)
    {
        var booking = await _repository.GetByReferenceAsync(
            bookingReference);

        return booking == null ? null : MapToResponse(booking);
    }

    // Create booking for the authenticated customer
    public async Task<BookingResponse> CreateAsync(
        CreateBookingRequest request,
        int customerId)
    {
        if (customerId <= 0)
        {
            throw new ArgumentException(
                "Invalid customer ID.");
        }

        if (request.SeatIds == null || request.SeatIds.Count == 0)
        {
            throw new ArgumentException(
                "At least one seat must be selected.");
        }

        if (request.SeatIds.Any(id => id <= 0))
        {
            throw new ArgumentException(
                "Seat IDs must be positive numbers.");
        }

        if (request.SeatIds.Distinct().Count() != request.SeatIds.Count)
        {
            throw new ArgumentException(
                "Duplicate seat IDs are not allowed.");
        }

        var booking = new Booking
        {
            BookingReference =
                "BK" + Guid.NewGuid().ToString("N")[..20].ToUpperInvariant(),

            ShowId = request.ShowId,

            // Ownership comes from the authenticated JWT
            CustomerId = customerId,

            CustomerName = request.CustomerName.Trim(),
            CustomerEmail = request.CustomerEmail.Trim(),
            CustomerPhone = request.CustomerPhone.Trim(),

            BookingDate = DateTime.UtcNow,
            Status = "Confirmed"
        };

        var createdBooking = await _repository.CreateAsync(
            booking,
            request.SeatIds);

        return MapToResponse(createdBooking);
    }

    // Admin/Manager: any booking
    // Customer: only their booking
    public async Task<CancelBookingResponse?> CancelAsync(
        int bookingId,
        int? customerId)
    {
        var booking = await _repository.CancelAsync(
            bookingId,
            customerId);

        if (booking == null)
        {
            return null;
        }

        return new CancelBookingResponse
        {
            BookingId = booking.BookingId,
            BookingReference = booking.BookingReference,
            Status = booking.Status,
            Message = "Booking cancelled successfully."
        };
    }

    private static BookingResponse MapToResponse(Booking booking)
    {
        return new BookingResponse
        {
            BookingId = booking.BookingId,
            BookingReference = booking.BookingReference,
            ShowId = booking.ShowId,
            CustomerName = booking.CustomerName,
            CustomerEmail = booking.CustomerEmail,
            CustomerPhone = booking.CustomerPhone,
            BookingDate = booking.BookingDate,
            TotalAmount = booking.TotalAmount,
            Status = booking.Status,
            SeatIds = booking.BookingSeats
                .Select(bs => bs.SeatId)
                .ToList()
        };
    }
}
