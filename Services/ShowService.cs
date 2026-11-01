
using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class ShowService : IShowService
{
    private readonly IShowRepository _repository;

    public ShowService(IShowRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ShowResponse>> GetAllAsync()
    {
        var shows = await _repository.GetAllAsync();

        return shows.Select(MapToResponse).ToList();
    }

    public async Task<ShowResponse?> GetByIdAsync(int id)
    {
        var show = await _repository.GetByIdAsync(id);

        return show == null ? null : MapToResponse(show);
    }

    public async Task<List<ShowResponse>> GetByMovieIdAsync(int movieId)
    {
        var shows = await _repository.GetByMovieIdAsync(movieId);

        return shows.Select(MapToResponse).ToList();
    }

    public async Task<List<ShowResponse>> GetByScreenIdAsync(int screenId)
    {
        var shows = await _repository.GetByScreenIdAsync(screenId);

        return shows.Select(MapToResponse).ToList();
    }

    public async Task<ShowResponse> CreateAsync(CreateShowRequest request)
    {
        ValidateShowTimings(request.StartTime, request.EndTime);

        if (request.TicketPrice < 0)
        {
            throw new ArgumentException("Ticket price cannot be negative.");
        }

        if (!await _repository.MovieExistsAsync(request.MovieId))
        {
            throw new ArgumentException("Movie not found.");
        }

        if (!await _repository.ScreenExistsAsync(request.ScreenId))
        {
            throw new ArgumentException("Screen not found.");
        }

        var show = new Show
        {
            MovieId = request.MovieId,
            ScreenId = request.ScreenId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TicketPrice = request.TicketPrice
        };

        var createdShow = await _repository.CreateAsync(show);

        return MapToResponse(createdShow);
    }

    public async Task<ShowResponse?> UpdateAsync(
        int id,
        UpdateShowRequest request)
    {
        var show = await _repository.GetByIdAsync(id);

        if (show == null)
        {
            return null;
        }

        ValidateShowTimings(request.StartTime, request.EndTime);

        if (request.TicketPrice < 0)
        {
            throw new ArgumentException("Ticket price cannot be negative.");
        }

        if (!await _repository.MovieExistsAsync(request.MovieId))
        {
            throw new ArgumentException("Movie not found.");
        }

        if (!await _repository.ScreenExistsAsync(request.ScreenId))
        {
            throw new ArgumentException("Screen not found.");
        }

        show.MovieId = request.MovieId;
        show.ScreenId = request.ScreenId;
        show.StartTime = request.StartTime;
        show.EndTime = request.EndTime;
        show.TicketPrice = request.TicketPrice;

        await _repository.UpdateAsync(show);

        var updatedShow = await _repository.GetByIdAsync(id);

        return updatedShow == null ? null : MapToResponse(updatedShow);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var show = await _repository.GetByIdAsync(id);

        if (show == null)
        {
            return false;
        }

        return await _repository.DeleteAsync(show);
    }

    private static void ValidateShowTimings(
        DateTime startTime,
        DateTime endTime)
    {
        if (startTime == default || endTime == default)
        {
            throw new ArgumentException("Start time and end time are required.");
        }

        if (endTime <= startTime)
        {
            throw new ArgumentException(
                "End time must be later than start time.");
        }
    }

    private static ShowResponse MapToResponse(Show show)
    {
        return new ShowResponse
        {
            ShowId = show.ShowId,
            MovieId = show.MovieId,
            MovieTitle = show.Movie.Title,
            ScreenId = show.ScreenId,
            ScreenName = show.Screen.ScreenName,
            StartTime = show.StartTime,
            EndTime = show.EndTime,
            TicketPrice = show.TicketPrice
        };
    }
}
