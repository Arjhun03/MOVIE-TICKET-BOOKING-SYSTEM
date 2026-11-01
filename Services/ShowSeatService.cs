using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class ShowSeatService : IShowSeatService
{
    private readonly IShowSeatRepository _repository;

    public ShowSeatService(IShowSeatRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ShowSeatResponse>> GetAllAsync()
    {
        var showSeats = await _repository.GetAllAsync();

        return showSeats.Select(MapToResponse);
    }

    public async Task<ShowSeatResponse?> GetByIdAsync(int id)
    {
        var showSeat = await _repository.GetByIdAsync(id);

        return showSeat == null ? null : MapToResponse(showSeat);
    }

    public async Task<IEnumerable<ShowSeatResponse>?> GetByShowIdAsync(int showId)
    {
        if (!await _repository.ShowExistsAsync(showId))
        {
            return null;
        }

        var showSeats = await _repository.GetByShowIdAsync(showId);

        return showSeats.Select(MapToResponse);
    }

    public async Task<IEnumerable<ShowSeatResponse>?> GetAvailableByShowIdAsync(int showId)
    {
        if (!await _repository.ShowExistsAsync(showId))
        {
            return null;
        }

        var showSeats = await _repository.GetAvailableByShowIdAsync(showId);

        return showSeats.Select(MapToResponse);
    }

    private static ShowSeatResponse MapToResponse(Models.ShowSeat showSeat)
    {
        return new ShowSeatResponse
        {
            ShowSeatId = showSeat.ShowSeatId,
            ShowId = showSeat.ShowId,
            SeatId = showSeat.SeatId,
            RowName = showSeat.Seat.RowName,
            SeatNumber = showSeat.Seat.SeatNumber,
            SeatType = showSeat.Seat.SeatType,
            Status = showSeat.Status
        };
    }
}