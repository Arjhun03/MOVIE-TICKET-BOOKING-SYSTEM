using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface IShowSeatRepository
{
    Task<IEnumerable<ShowSeat>> GetAllAsync();

    Task<ShowSeat?> GetByIdAsync(int id);

    Task<IEnumerable<ShowSeat>> GetByShowIdAsync(int showId);

    Task<IEnumerable<ShowSeat>> GetAvailableByShowIdAsync(int showId);

    Task<bool> ShowExistsAsync(int showId);
}