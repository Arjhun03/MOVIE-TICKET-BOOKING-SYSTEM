
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class SeatService : ISeatService
{
    private readonly ISeatRepository _repository;

    public SeatService(ISeatRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<SeatResponse>> GetAllAsync()
    {
        var seats = await _repository.GetAllAsync();

        return seats.Select(MapToResponse).ToList();
    }

    public async Task<SeatResponse?> GetByIdAsync(int id)
    {
        var seat = await _repository.GetByIdAsync(id);

        return seat == null ? null : MapToResponse(seat);
    }

    public async Task<List<SeatResponse>?> GetByScreenIdAsync(int screenId)
    {
        var seats = await _repository.GetByScreenIdAsync(screenId);

        return seats?.Select(MapToResponse).ToList();
    }

    public async Task<SeatResponse> CreateAsync(CreateSeatRequest request)
    {
        if (!await _repository.ScreenExistsAsync(request.ScreenId))
        {
            throw new ArgumentException("Screen not found.");
        }

        string rowName = request.RowName.Trim().ToUpperInvariant();

        if (await _repository.SeatExistsAsync(
                request.ScreenId, rowName, request.SeatNumber))
        {
            throw new ArgumentException(
                "This seat already exists in the selected screen.");
        }

        var seat = new Seat
        {
            ScreenId = request.ScreenId,
            RowName = rowName,
            SeatNumber = request.SeatNumber,
            SeatType = request.SeatType.Trim()
        };

        var createdSeat = await _repository.CreateAsync(seat);

        return MapToResponse(createdSeat);
    }

    public async Task<List<SeatResponse>> CreateBulkAsync(
        BulkCreateSeatsRequest request)
    {
        if (!await _repository.ScreenExistsAsync(request.ScreenId))
        {
            throw new ArgumentException("Screen not found.");
        }

        var normalizedSeats = request.Seats.Select(s => new
        {
            RowName = s.RowName.Trim().ToUpperInvariant(),
            s.SeatNumber,
            SeatType = s.SeatType.Trim()
        }).ToList();

        // Check duplicates within the same request
        bool hasRequestDuplicates = normalizedSeats
            .GroupBy(s => new { s.RowName, s.SeatNumber })
            .Any(g => g.Count() > 1);

        if (hasRequestDuplicates)
        {
            throw new ArgumentException(
                "The request contains duplicate seat positions.");
        }

        // Check whether any requested seat already exists
        foreach (var item in normalizedSeats)
        {
            if (await _repository.SeatExistsAsync(
                    request.ScreenId,
                    item.RowName,
                    item.SeatNumber))
            {
                throw new ArgumentException(
                    $"Seat {item.RowName}{item.SeatNumber} already exists.");
            }
        }

        var seats = normalizedSeats.Select(s => new Seat
        {
            ScreenId = request.ScreenId,
            RowName = s.RowName,
            SeatNumber = s.SeatNumber,
            SeatType = s.SeatType
        }).ToList();

        var createdSeats = await _repository.CreateBulkAsync(seats);

        return createdSeats.Select(MapToResponse).ToList();
    }

    public async Task<SeatResponse?> UpdateAsync(
        int id,
        UpdateSeatRequest request)
    {
        var seat = await _repository.GetByIdAsync(id);

        if (seat == null)
        {
            return null;
        }

        if (!await _repository.ScreenExistsAsync(request.ScreenId))
        {
            throw new ArgumentException("Screen not found.");
        }

        string rowName = request.RowName.Trim().ToUpperInvariant();

        if (await _repository.SeatExistsAsync(
                request.ScreenId,
                rowName,
                request.SeatNumber,
                id))
        {
            throw new ArgumentException(
                "This seat position already exists in the selected screen.");
        }

        seat.ScreenId = request.ScreenId;
        seat.RowName = rowName;
        seat.SeatNumber = request.SeatNumber;
        seat.SeatType = request.SeatType.Trim();

        await _repository.UpdateAsync(seat);

        var updatedSeat = await _repository.GetByIdAsync(id);

        return updatedSeat == null ? null : MapToResponse(updatedSeat);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var seat = await _repository.GetByIdAsync(id);

        if (seat == null)
        {
            return false;
        }

        return await _repository.DeleteAsync(seat);
    }

    private static SeatResponse MapToResponse(Seat seat)
    {
        return new SeatResponse
        {
            SeatId = seat.SeatId,
            ScreenId = seat.ScreenId,
            ScreenName = seat.Screen.ScreenName,
            RowName = seat.RowName,
            SeatNumber = seat.SeatNumber,
            SeatType = seat.SeatType
        };
    }
}
