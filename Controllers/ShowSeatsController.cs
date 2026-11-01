using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowSeatsController : ControllerBase
{
    private readonly IShowSeatService _service;

    public ShowSeatsController(IShowSeatService service)
    {
        _service = service;
    }

    // GET: api/showseats
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var showSeats = await _service.GetAllAsync();

        return Ok(showSeats);
    }

    // GET: api/showseats/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var showSeat = await _service.GetByIdAsync(id);

        if (showSeat == null)
        {
            return NotFound(new { message = "Show seat not found." });
        }

        return Ok(showSeat);
    }

    // GET: api/showseats/show/1
    [HttpGet("show/{showId:int}")]
    public async Task<IActionResult> GetByShowId(int showId)
    {
        var showSeats = await _service.GetByShowIdAsync(showId);

        if (showSeats == null)
        {
            return NotFound(new { message = "Show not found." });
        }

        return Ok(showSeats);
    }

    // GET: api/showseats/show/1/available
    [HttpGet("show/{showId:int}/available")]
    public async Task<IActionResult> GetAvailableByShowId(int showId)
    {
        var showSeats = await _service.GetAvailableByShowIdAsync(showId);

        if (showSeats == null)
        {
            return NotFound(new { message = "Show not found." });
        }

        return Ok(showSeats);
    }
}