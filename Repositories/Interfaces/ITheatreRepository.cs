using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface ITheatreRepository
{
    Task<List<Theatre>> GetAllAsync();

    Task<Theatre?> GetByIdAsync(int id);

    Task<Theatre> CreateAsync(Theatre theatre);

    Task<Theatre?> UpdateAsync(int id, Theatre theatre);

    Task<bool> DeleteAsync(int id);
}