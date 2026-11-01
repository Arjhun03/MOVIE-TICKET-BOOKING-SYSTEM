using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class ShowSeatRepository : IShowSeatRepository
{
    private readonly ApplicationDbContext _context;

    public ShowSeatRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ShowSeat>> GetAllAsync()
    {
        return await _context.ShowSeats
            .Include(ss => ss.Seat)
            .OrderBy(ss => ss.ShowId)
            .ThenBy(ss => ss.Seat.RowName)
            .ThenBy(ss => ss.Seat.SeatNumber)
            .ToListAsync();
    }

    public async Task<ShowSeat?> GetByIdAsync(int id)
    {
        return await _context.ShowSeats
            .Include(ss => ss.Seat)
            .FirstOrDefaultAsync(ss => ss.ShowSeatId == id);
    }

    public async Task<IEnumerable<ShowSeat>> GetByShowIdAsync(int showId)
    {
        return await _context.ShowSeats
            .Include(ss => ss.Seat)
            .Where(ss => ss.ShowId == showId)
            .OrderBy(ss => ss.Seat.RowName)
            .ThenBy(ss => ss.Seat.SeatNumber)
            .ToListAsync();
    }

    public async Task<IEnumerable<ShowSeat>> GetAvailableByShowIdAsync(int showId)
    {
        return await _context.ShowSeats
            .Include(ss => ss.Seat)
            .Where(ss => ss.ShowId == showId && ss.Status == "Available")
            .OrderBy(ss => ss.Seat.RowName)
            .ThenBy(ss => ss.Seat.SeatNumber)
            .ToListAsync();
    }

    public async Task<bool> ShowExistsAsync(int showId)
    {
        return await _context.Shows.AnyAsync(s => s.ShowId == showId);
    }
}