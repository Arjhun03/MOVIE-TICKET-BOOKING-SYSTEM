
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
            .ToListAsync();
    }

    public async Task<List<Show>> GetByScreenIdAsync(int screenId)
    {
        return await _context.Shows
            .Include(s => s.Movie)
            .Include(s => s.Screen)
            .Where(s => s.ScreenId == screenId)
            .ToListAsync();
    }

    public async Task<Show> CreateAsync(Show show)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        _context.Shows.Add(show);

        await _context.SaveChangesAsync();

        // Get all physical seats belonging to this screen.
        var seats = await _context.Seats
            .Where(s => s.ScreenId == show.ScreenId)
            .ToListAsync();

        // Generate ShowSeat records automatically.
        var showSeats = seats.Select(seat => new ShowSeat
        {
            ShowId = show.ShowId,
            SeatId = seat.SeatId,
            Status = "Available"
        }).ToList();

        _context.ShowSeats.AddRange(showSeats);

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        // Reload Show with Movie and Screen details.
        var createdShow = await _context.Shows
            .Include(s => s.Movie)
            .Include(s => s.Screen)
            .FirstAsync(s => s.ShowId == show.ShowId);

        return createdShow;
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
