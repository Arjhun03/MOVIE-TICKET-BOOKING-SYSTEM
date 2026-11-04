
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface IBookingRepository
{
    // Admin/Manager: all bookings
    // Customer: own bookings
    Task<List<Booking>> GetAllAsync(int? customerId);

    // Admin/Manager: any booking
    // Customer: own booking
    Task<Booking?> GetByIdAsync(
        int bookingId,
        int? customerId);

    Task<Booking?> GetByReferenceAsync(
        string bookingReference);

    Task<Booking> CreateAsync(
        Booking booking,
        List<int> seatIds);

    // Admin/Manager: any booking
    // Customer: own booking
    Task<Booking?> CancelAsync(
        int bookingId,
        int? customerId);
}
