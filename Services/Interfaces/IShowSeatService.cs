using MovieTicketBooking.DTOs.Shows;

namespace MovieTicketBooking.Services.Interfaces;

public interface IShowSeatService
{
    Task<IEnumerable<ShowSeatResponse>> GetAllAsync();

    Task<ShowSeatResponse?> GetByIdAsync(int id);

    Task<IEnumerable<ShowSeatResponse>?> GetByShowIdAsync(int showId);

    Task<IEnumerable<ShowSeatResponse>?> GetAvailableByShowIdAsync(int showId);
}