using MovieTicketBooking.DTOs.Screens;

namespace MovieTicketBooking.Services.Interfaces;

public interface IScreenService
{
    Task<List<ScreenResponse>> GetAllAsync();

    Task<ScreenResponse?> GetByIdAsync(int id);

    Task<List<ScreenResponse>?> GetByTheatreIdAsync(int theatreId);

    Task<ScreenResponse> CreateAsync(CreateScreenRequest request);

    Task<ScreenResponse?> UpdateAsync(int id, UpdateScreenRequest request);

    Task<bool> DeleteAsync(int id);
}