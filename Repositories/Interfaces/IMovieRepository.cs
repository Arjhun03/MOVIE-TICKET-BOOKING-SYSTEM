using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface IMovieRepository
{
    Task<List<Movie>> GetAllAsync(int pageNumber, int pageSize);

    Task<Movie?> GetByIdAsync(int id);

    Task<Movie> CreateAsync(Movie movie);

    Task<Movie?> UpdateAsync(int id, Movie movie);

    Task<bool> DeleteAsync(int id);

    Task<List<Movie>> SearchByTitleAsync(string title);
}