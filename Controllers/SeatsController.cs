
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeatsController : ControllerBase
{
    private readonly ISeatService _service;

    public SeatsController(ISeatService service)
    {
        _service = service;
    }

    // GET: api/seats
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var seats = await _service.GetAllAsync();

        return Ok(seats);
    }

    // GET: api/seats/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var seat = await _service.GetByIdAsync(id);

        if (seat == null)
        {
            return NotFound(new { message = "Seat not found." });
        }

        return Ok(seat);
    }

    // GET: api/seats/screen/1
    [HttpGet("screen/{screenId:int}")]
    public async Task<IActionResult> GetByScreenId(int screenId)
    {
        var seats = await _service.GetByScreenIdAsync(screenId);

        if (seats == null)
        {
            return NotFound(new { message = "Screen not found." });
        }

        return Ok(seats);
    }

    // POST: api/seats
    [HttpPost]
    public async Task<IActionResult> Create(CreateSeatRequest request)
    {
        try
        {
            var seat = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = seat.SeatId },
                seat);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/seats/bulk
    [HttpPost("bulk")]
    public async Task<IActionResult> CreateBulk(
        BulkCreateSeatsRequest request)
    {
        try
        {
            var seats = await _service.CreateBulkAsync(request);

            return Ok(seats);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/seats/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateSeatRequest request)
    {
        try
        {
            var seat = await _service.UpdateAsync(id, request);

            if (seat == null)
            {
                return NotFound(new { message = "Seat not found." });
            }

            return Ok(seat);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/seats/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _service.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new { message = "Seat not found." });
        }

        return NoContent();
    }
}
