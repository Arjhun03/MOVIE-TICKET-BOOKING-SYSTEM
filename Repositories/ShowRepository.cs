
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class ShowRepository : IShowRepository
{
    private readonly ApplicationDbContext _context;

    public ShowRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Show>> GetAllAsync()
    {
        return await _context.Shows
            .Include(s => s.Movie)
            .Include(s => s.Screen)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<Show?> GetByIdAsync(int id)
    {
        return await _context.Shows
            .Include(s => s.Movie)
            .Include(s => s.Screen)
            .FirstOrDefaultAsync(s => s.ShowId == id);
    }

    public async Task<List<Show>> GetByMovieIdAsync(int movieId)
    {
        return await _context.Shows
            .Include(s => s.Movie)
            .Include(s => s.Screen)
            .Where(s => s.MovieId == movieId)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<List<Show>> GetByScreenIdAsync(int screenId)
    {
        return await _context.Shows
            .Include(s => s.Movie)
            .Include(s => s.Screen)
            .Where(s => s.ScreenId == screenId)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<Show> CreateAsync(Show show)
    {
        _context.Shows.Add(show);

        await _context.SaveChangesAsync();

        return await GetByIdAsync(show.ShowId) ?? show;
    }

    public async Task<bool> UpdateAsync(Show show)
    {
        _context.Shows.Update(show);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Show show)
    {
        _context.Shows.Remove(show);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> MovieExistsAsync(int movieId)
    {
        return await _context.Movies
            .AnyAsync(m => m.MovieId == movieId);
    }

    public async Task<bool> ScreenExistsAsync(int screenId)
    {
        return await _context.Screens
            .AnyAsync(s => s.ScreenId == screenId);
    }
}
