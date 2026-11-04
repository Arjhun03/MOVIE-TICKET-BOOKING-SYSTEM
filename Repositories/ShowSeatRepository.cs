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
        var showSeats = await _context.ShowSeats
            .Include(ss => ss.Seat)
            .Where(ss => ss.ShowId == showId)
            .OrderBy(ss => ss.Seat.RowName)
            .ThenBy(ss => ss.Seat.SeatNumber)
            .ToListAsync();

        if (showSeats.Count == 0)
        {
            var show = await _context.Shows.FirstOrDefaultAsync(s => s.ShowId == showId);
            if (show != null)
            {
                var screenSeats = await _context.Seats
                    .Where(s => s.ScreenId == show.ScreenId)
                    .OrderBy(s => s.RowName)
                    .ThenBy(s => s.SeatNumber)
                    .ToListAsync();

                if (screenSeats.Count == 0)
                {
                    // Generate 25 default seats (5 rows x 5 seats: A1-A5 to E1-E5)
                    var rows = new[] { "A", "B", "C", "D", "E" };
                    for (int r = 0; r < rows.Length; r++)
                    {
                        var rowName = rows[r];
                        var type = r == 0 ? "VIP" : (r <= 2 ? "Premium" : "Regular");
                        for (int num = 1; num <= 5; num++)
                        {
                            var seat = new Seat
                            {
                                ScreenId = show.ScreenId,
                                RowName = rowName,
                                SeatNumber = num,
                                SeatType = type
                            };
                            _context.Seats.Add(seat);
                            screenSeats.Add(seat);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                foreach (var seat in screenSeats.Take(25))
                {
                    var showSeat = new ShowSeat
                    {
                        ShowId = showId,
                        SeatId = seat.SeatId,
                        Status = "Available"
                    };
                    _context.ShowSeats.Add(showSeat);
                }
                await _context.SaveChangesAsync();

                showSeats = await _context.ShowSeats
                    .Include(ss => ss.Seat)
                    .Where(ss => ss.ShowId == showId)
                    .OrderBy(ss => ss.Seat.RowName)
                    .ThenBy(ss => ss.Seat.SeatNumber)
                    .ToListAsync();
            }
        }

        return showSeats;
    }

    public async Task<IEnumerable<ShowSeat>> GetAvailableByShowIdAsync(int showId)
    {
        await GetByShowIdAsync(showId);

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