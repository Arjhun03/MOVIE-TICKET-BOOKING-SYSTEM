
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowsController : ControllerBase
{
    private readonly IShowService _service;

    public ShowsController(IShowService service)
    {
        _service = service;
    }

    // GET: api/shows
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var shows = await _service.GetAllAsync();

        return Ok(shows);
    }

    // GET: api/shows/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var show = await _service.GetByIdAsync(id);

        if (show == null)
        {
            return NotFound(new { message = "Show not found." });
        }

        return Ok(show);
    }

    // GET: api/shows/movie/1
    [HttpGet("movie/{movieId:int}")]
    public async Task<IActionResult> GetByMovieId(int movieId)
    {
        var shows = await _service.GetByMovieIdAsync(movieId);

        return Ok(shows);
    }

    // GET: api/shows/screen/1
    [HttpGet("screen/{screenId:int}")]
    public async Task<IActionResult> GetByScreenId(int screenId)
    {
        var shows = await _service.GetByScreenIdAsync(screenId);

        return Ok(shows);
    }

    // POST: api/shows
    [HttpPost]
    public async Task<IActionResult> Create(CreateShowRequest request)
    {
        try
        {
            var show = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = show.ShowId },
                show);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/shows/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateShowRequest request)
    {
        try
        {
            var show = await _service.UpdateAsync(id, request);

            if (show == null)
            {
                return NotFound(new { message = "Show not found." });
            }

            return Ok(show);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/shows/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _service.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new { message = "Show not found." });
        }

        return NoContent();
    }
}
