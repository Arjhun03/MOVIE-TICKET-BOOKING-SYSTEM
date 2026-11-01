using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface IShowRepository
{
    Task<List<Show>> GetAllAsync();

    Task<Show?> GetByIdAsync(int id);

    Task<List<Show>> GetByMovieIdAsync(int movieId);

    Task<List<Show>> GetByScreenIdAsync(int screenId);

    Task<Show> CreateAsync(Show show);

    Task<bool> UpdateAsync(Show show);

    Task<bool> DeleteAsync(Show show);

    Task<bool> MovieExistsAsync(int movieId);

    Task<bool> ScreenExistsAsync(int screenId);
}