using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface ISeatRepository
{
    Task<List<Seat>> GetAllAsync();

    Task<Seat?> GetByIdAsync(int id);

    Task<List<Seat>?> GetByScreenIdAsync(int screenId);

    Task<Seat> CreateAsync(Seat seat);

    Task<List<Seat>> CreateBulkAsync(List<Seat> seats);

    Task<bool> UpdateAsync(Seat seat);

    Task<bool> DeleteAsync(Seat seat);

    Task<bool> ScreenExistsAsync(int screenId);

    Task<bool> SeatExistsAsync(
        int screenId,
        string rowName,
        int seatNumber,
        int? excludeSeatId = null);
}
