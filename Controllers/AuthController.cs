using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Auth;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService userService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await userService.RegisterAsync(dto);

        if (!result.Success)
        {
            return BadRequest(new
            {
                message = result.Message
            });
        }

        return Ok(new
        {
            message = result.Message
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await userService.LoginAsync(dto);

        if (!result.Success)
        {
            return Unauthorized(new
            {
                message = result.Message
            });
        }

        return Ok(new
        {
            message = result.Message,
            token = result.Token
        });
    }
}