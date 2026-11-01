using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllAsync();

    Task<Booking?> GetByIdAsync(int bookingId);

    Task<Booking?> GetByReferenceAsync(string bookingReference);

    Task<Booking> CreateAsync(Booking booking, List<int> seatIds);

    Task<Booking?> CancelAsync(int bookingId);
}