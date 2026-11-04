
using Microsoft.AspNetCore.Identity;
using MovieTicketBooking.DTOs.Auth;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class UserService(
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher,
    IJwtService jwtService
) : IUserService
{
    public async Task<AuthResponse> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var existingUser = await userRepository.GetByEmailAsync(email);

        if (existingUser != null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Email is already registered."
            };
        }

        var user = new User
        {
            FullName = dto.FullName.Trim(),
            Email = email,
            Role = "Customer"
        };

        user.PasswordHash = passwordHasher.HashPassword(
            user,
            dto.Password);

        await userRepository.AddAsync(user);
        await userRepository.SaveChangesAsync();

        return new AuthResponse
        {
            Success = true,
            Message = "Registration successful."
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var user = await userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid email or password."
            };
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid email or password."
            };
        }

        var token = jwtService.GenerateToken(user);

        return new AuthResponse
        {
            Success = true,
            Message = "Login successful.",
            Token = token
        };
        
        
        
    }
}
