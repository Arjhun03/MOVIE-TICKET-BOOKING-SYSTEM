
using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class TheatreService : ITheatreService
{
    private readonly ITheatreRepository _repository;

    public TheatreService(ITheatreRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TheatreResponse>> GetAllAsync()
    {
        var theatres = await _repository.GetAllAsync();

        return theatres.Select(theatre => new TheatreResponse
        {
            TheatreId = theatre.TheatreId,
            Name = theatre.Name,
            Location = theatre.Location
        }).ToList();
    }

    public async Task<TheatreResponse?> GetByIdAsync(int id)
    {
        var theatre = await _repository.GetByIdAsync(id);

        if (theatre == null)
        {
            return null;
        }

        return new TheatreResponse
        {
            TheatreId = theatre.TheatreId,
            Name = theatre.Name,
            Location = theatre.Location
        };
    }

    public async Task<TheatreResponse> CreateAsync(CreateTheatreRequest request)
    {
        var theatre = new Theatre
        {
            Name = request.Name,
            Location = request.Location
        };

        var createdTheatre = await _repository.CreateAsync(theatre);

        return new TheatreResponse
        {
            TheatreId = createdTheatre.TheatreId,
            Name = createdTheatre.Name,
            Location = createdTheatre.Location
        };
    }

    public async Task<TheatreResponse?> UpdateAsync(int id, UpdateTheatreRequest request)
    {
        var theatre = new Theatre
        {
            Name = request.Name,
            Location = request.Location
        };

        var updatedTheatre = await _repository.UpdateAsync(id, theatre);

        if (updatedTheatre == null)
        {
            return null;
        }

        return new TheatreResponse
        {
            TheatreId = updatedTheatre.TheatreId,
            Name = updatedTheatre.Name,
            Location = updatedTheatre.Location
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}
