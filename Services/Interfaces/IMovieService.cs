using MovieTicketBooking.DTOs.Movies;

namespace MovieTicketBooking.Services.Interfaces;

public interface IMovieService
{
    Task<List<MovieResponse>> GetAllAsync(
        int pageNumber, int pageSize);

    Task<MovieResponse?> GetByIdAsync(int id);

    Task<MovieResponse> CreateAsync(CreateMovieRequest request);

    Task<MovieResponse?> UpdateAsync(
        int id, UpdateMovieRequest request);

    Task<bool> DeleteAsync(int id);

    Task<List<MovieResponse>> SearchByTitleAsync(string title);
}