
using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class ScreenService : IScreenService
{
    private readonly IScreenRepository _repository;

    public ScreenService(IScreenRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ScreenResponse>> GetAllAsync()
    {
        var screens = await _repository.GetAllAsync();

        return screens.Select(MapToResponse).ToList();
    }

    public async Task<ScreenResponse?> GetByIdAsync(int id)
    {
        var screen = await _repository.GetByIdAsync(id);

        if (screen == null)
        {
            return null;
        }

        return MapToResponse(screen);
    }

    public async Task<List<ScreenResponse>?> GetByTheatreIdAsync(
        int theatreId)
    {
        var exists = await _repository.TheatreExistsAsync(theatreId);

        if (!exists)
        {
            return null;
        }

        var screens = await _repository.GetByTheatreIdAsync(theatreId);

        return screens.Select(MapToResponse).ToList();
    }

    public async Task<ScreenResponse> CreateAsync(
        CreateScreenRequest request)
    {
        var theatreExists = await _repository.TheatreExistsAsync(
            request.TheatreId);

        if (!theatreExists)
        {
            throw new ArgumentException("Theatre not found.");
        }

        var screen = new Screen
        {
            TheatreId = request.TheatreId,
            ScreenName = request.ScreenName
        };

        var createdScreen = await _repository.CreateAsync(screen);

        return MapToResponse(createdScreen);
    }

    public async Task<ScreenResponse?> UpdateAsync(
        int id, UpdateScreenRequest request)
    {
        var theatreExists = await _repository.TheatreExistsAsync(
            request.TheatreId);

        if (!theatreExists)
        {
            throw new ArgumentException("Theatre not found.");
        }

        var screen = new Screen
        {
            TheatreId = request.TheatreId,
            ScreenName = request.ScreenName
        };

        var updatedScreen = await _repository.UpdateAsync(id, screen);

        if (updatedScreen == null)
        {
            return null;
        }

        return MapToResponse(updatedScreen);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    private static ScreenResponse MapToResponse(Screen screen)
    {
        return new ScreenResponse
        {
            ScreenId = screen.ScreenId,
            TheatreId = screen.TheatreId,
            TheatreName = screen.Theatre.Name,
            ScreenName = screen.ScreenName
        };
    }
}
