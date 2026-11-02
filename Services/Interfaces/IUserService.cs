using MovieTicketBooking.DTOs.Auth;

namespace MovieTicketBooking.Services.Interfaces;

public interface IUserService
{
    Task<AuthResponse> RegisterAsync(RegisterDto dto);

    Task<AuthResponse> LoginAsync(LoginDto dto);
}