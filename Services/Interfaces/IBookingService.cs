
using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interfaces;

public interface IBookingService
{
    // Admin/Manager: all bookings
    // Customer: only their own bookings
    Task<List<BookingResponse>> GetAllAsync(int? customerId);

    // Admin/Manager: any booking
    // Customer: only their own booking
    Task<BookingResponse?> GetByIdAsync(
        int bookingId,
        int? customerId);

    Task<BookingResponse?> GetByReferenceAsync(
        string bookingReference);

    // Always requires an authenticated customer ID
    Task<BookingResponse> CreateAsync(
        CreateBookingRequest request,
        int customerId);

    // Admin/Manager: any booking
    // Customer: only their own booking
    Task<CancelBookingResponse?> CancelAsync(
        int bookingId,
        int? customerId);
}
