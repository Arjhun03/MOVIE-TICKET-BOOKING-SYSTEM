using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interfaces;

public interface IBookingService
{
    Task<List<BookingResponse>> GetAllAsync();

    Task<BookingResponse?> GetByIdAsync(int bookingId);

    Task<BookingResponse?> GetByReferenceAsync(string bookingReference);

    Task<BookingResponse> CreateAsync(CreateBookingRequest request);

    Task<CancelBookingResponse?> CancelAsync(int bookingId);
}