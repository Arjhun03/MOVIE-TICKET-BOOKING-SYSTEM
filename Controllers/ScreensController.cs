
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScreensController : ControllerBase
{
    private readonly IScreenService _service;

    public ScreensController(IScreenService service)
    {
        _service = service;
    }

    // GET: api/screens
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var screens = await _service.GetAllAsync();

        return Ok(screens);
    }

    // GET: api/screens/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var screen = await _service.GetByIdAsync(id);

        if (screen == null)
        {
            return NotFound(new
            {
                message = "Screen not found"
            });
        }

        return Ok(screen);
    }

    // GET: api/screens/theatre/1
    [HttpGet("theatre/{theatreId:int}")]
    public async Task<IActionResult> GetByTheatreId(int theatreId)
    {
        var screens = await _service.GetByTheatreIdAsync(theatreId);

        if (screens == null)
        {
            return NotFound(new
            {
                message = "Theatre not found"
            });
        }

        return Ok(screens);
    }

    // POST: api/screens
    [HttpPost]
    public async Task<IActionResult> Create(CreateScreenRequest request)
    {
        try
        {
            var screen = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = screen.ScreenId },
                screen
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // PUT: api/screens/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, UpdateScreenRequest request)
    {
        try
        {
            var screen = await _service.UpdateAsync(id, request);

            if (screen == null)
            {
                return NotFound(new
                {
                    message = "Screen not found"
                });
            }

            return Ok(screen);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // DELETE: api/screens/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Screen not found"
            });
        }

        return NoContent();
    }
}
