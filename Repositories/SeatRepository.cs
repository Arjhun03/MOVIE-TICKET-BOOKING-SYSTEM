
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class SeatRepository : ISeatRepository
{
    private readonly ApplicationDbContext _context;

    public SeatRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Seat>> GetAllAsync()
    {
        return await _context.Seats
            .Include(s => s.Screen)
            .OrderBy(s => s.ScreenId)
            .ThenBy(s => s.RowName)
            .ThenBy(s => s.SeatNumber)
            .ToListAsync();
    }

    public async Task<Seat?> GetByIdAsync(int id)
    {
        return await _context.Seats
            .Include(s => s.Screen)
            .FirstOrDefaultAsync(s => s.SeatId == id);
    }

    public async Task<List<Seat>?> GetByScreenIdAsync(int screenId)
    {
        bool screenExists = await _context.Screens
            .AnyAsync(s => s.ScreenId == screenId);

        if (!screenExists)
        {
            return null;
        }

        return await _context.Seats
            .Include(s => s.Screen)
            .Where(s => s.ScreenId == screenId)
            .OrderBy(s => s.RowName)
            .ThenBy(s => s.SeatNumber)
            .ToListAsync();
    }

    public async Task<Seat> CreateAsync(Seat seat)
    {
        _context.Seats.Add(seat);

        await _context.SaveChangesAsync();

        return await GetByIdAsync(seat.SeatId) ?? seat;
    }

    public async Task<List<Seat>> CreateBulkAsync(List<Seat> seats)
    {
        await _context.Seats.AddRangeAsync(seats);

        await _context.SaveChangesAsync();

        return await _context.Seats
            .Include(s => s.Screen)
            .Where(s => seats.Select(x => x.SeatId).Contains(s.SeatId))
            .OrderBy(s => s.RowName)
            .ThenBy(s => s.SeatNumber)
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(Seat seat)
    {
        _context.Seats.Update(seat);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Seat seat)
    {
        _context.Seats.Remove(seat);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ScreenExistsAsync(int screenId)
    {
        return await _context.Screens
            .AnyAsync(s => s.ScreenId == screenId);
    }

    public async Task<bool> SeatExistsAsync(
        int screenId,
        string rowName,
        int seatNumber,
        int? excludeSeatId = null)
    {
        return await _context.Seats.AnyAsync(s =>
            s.ScreenId == screenId &&
            s.RowName == rowName &&
            s.SeatNumber == seatNumber &&
            (!excludeSeatId.HasValue || s.SeatId != excludeSeatId.Value));
    }
}

