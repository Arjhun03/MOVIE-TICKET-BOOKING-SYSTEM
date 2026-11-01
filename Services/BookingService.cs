
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

    public async Task<List<BookingResponse>> GetAllAsync()
    {
        var bookings = await _repository.GetAllAsync();

        return bookings.Select(MapToResponse).ToList();
    }

    public async Task<BookingResponse?> GetByIdAsync(int bookingId)
    {
        var booking = await _repository.GetByIdAsync(bookingId);

        return booking == null ? null : MapToResponse(booking);
    }

    public async Task<BookingResponse?> GetByReferenceAsync(
        string bookingReference)
    {
        var booking = await _repository.GetByReferenceAsync(
            bookingReference);

        return booking == null ? null : MapToResponse(booking);
    }

    public async Task<BookingResponse> CreateAsync(
        CreateBookingRequest request)
    {
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

    public async Task<CancelBookingResponse?> CancelAsync(int bookingId)
    {
        var booking = await _repository.CancelAsync(bookingId);

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
