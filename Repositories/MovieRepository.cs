
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly ApplicationDbContext _context;

    public MovieRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Movie>> GetAllAsync(
        int pageNumber, int pageSize)
    {
        return await _context.Movies
            .OrderBy(m => m.MovieId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Movie?> GetByIdAsync(int id)
    {
        return await _context.Movies.FindAsync(id);
    }

    public async Task<Movie> CreateAsync(Movie movie)
    {
        _context.Movies.Add(movie);

        await _context.SaveChangesAsync();

        return movie;
    }

    public async Task<Movie?> UpdateAsync(int id, Movie movie)
    {
        var existingMovie = await _context.Movies.FindAsync(id);

        if (existingMovie == null)
        {
            return null;
        }

        existingMovie.Title = movie.Title;
        existingMovie.Description = movie.Description;
        existingMovie.DurationMinutes = movie.DurationMinutes;
        existingMovie.ReleaseDate = movie.ReleaseDate;
        existingMovie.Language = movie.Language;
        existingMovie.Genre = movie.Genre;

        await _context.SaveChangesAsync();

        return existingMovie;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return false;
        }

        _context.Movies.Remove(movie);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<Movie>> SearchByTitleAsync(string title)
    {
        return await _context.Movies
            .Where(m => m.Title.Contains(title))
            .ToListAsync();
    }
}
