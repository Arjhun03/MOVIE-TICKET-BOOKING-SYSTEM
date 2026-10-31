
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _service;

    public MoviesController(IMovieService service)
    {
        _service = service;
    }

    // GET: api/movies
    [HttpGet]
    public async Task<IActionResult> GetAll(
        int pageNumber = 1,
        int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new
            {
                message = "Page number must be positive and page size must be between 1 and 100."
            });
        }

        var movies = await _service.GetAllAsync(
            pageNumber, pageSize);

        return Ok(movies);
    }

    // GET: api/movies/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var movie = await _service.GetByIdAsync(id);

        if (movie == null)
        {
            return NotFound(new
            {
                message = "Movie not found"
            });
        }

        return Ok(movie);
    }

    // GET: api/movies/search?title=GBU
    [HttpGet("search")]
    public async Task<IActionResult> Search(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return BadRequest(new
            {
                message = "Movie title is required"
            });
        }

        var movies = await _service.SearchByTitleAsync(title);

        return Ok(movies);
    }

    // POST: api/movies
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateMovieRequest request)
    {
        var movie = await _service.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = movie.MovieId },
            movie
        );
    }

    // PUT: api/movies/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateMovieRequest request)
    {
        var movie = await _service.UpdateAsync(id, request);

        if (movie == null)
        {
            return NotFound(new
            {
                message = "Movie not found"
            });
        }

        return Ok(movie);
    }

    // DELETE: api/movies/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Movie not found"
            });
        }

        return NoContent();
    }
}
