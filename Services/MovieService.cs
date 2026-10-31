
using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _repository;

    public MovieService(IMovieRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MovieResponse>> GetAllAsync(
        int pageNumber, int pageSize)
    {
        var movies = await _repository.GetAllAsync(
            pageNumber, pageSize);

        return movies.Select(MapToResponse).ToList();
    }

    public async Task<MovieResponse?> GetByIdAsync(int id)
    {
        var movie = await _repository.GetByIdAsync(id);

        if (movie == null)
        {
            return null;
        }

        return MapToResponse(movie);
    }

    public async Task<MovieResponse> CreateAsync(
        CreateMovieRequest request)
    {
        var movie = new Movie
        {
            Title = request.Title,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            ReleaseDate = request.ReleaseDate,
            Language = request.Language,
            Genre = request.Genre
        };

        var createdMovie = await _repository.CreateAsync(movie);

        return MapToResponse(createdMovie);
    }

    public async Task<MovieResponse?> UpdateAsync(
        int id, UpdateMovieRequest request)
    {
        var movie = new Movie
        {
            Title = request.Title,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            ReleaseDate = request.ReleaseDate,
            Language = request.Language,
            Genre = request.Genre
        };

        var updatedMovie = await _repository.UpdateAsync(id, movie);

        if (updatedMovie == null)
        {
            return null;
        }

        return MapToResponse(updatedMovie);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<List<MovieResponse>> SearchByTitleAsync(
        string title)
    {
        var movies = await _repository.SearchByTitleAsync(title);

        return movies.Select(MapToResponse).ToList();
    }

    private static MovieResponse MapToResponse(Movie movie)
    {
        return new MovieResponse
        {
            MovieId = movie.MovieId,
            Title = movie.Title,
            Description = movie.Description,
            DurationMinutes = movie.DurationMinutes,
            ReleaseDate = movie.ReleaseDate,
            Language = movie.Language,
            Genre = movie.Genre
        };
    }
}
