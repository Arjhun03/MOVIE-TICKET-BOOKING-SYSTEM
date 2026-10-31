
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class ScreenRepository : IScreenRepository
{
    private readonly ApplicationDbContext _context;

    public ScreenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Screen>> GetAllAsync()
    {
        return await _context.Screens
            .Include(s => s.Theatre)
            .ToListAsync();
    }

    public async Task<Screen?> GetByIdAsync(int id)
    {
        return await _context.Screens
            .Include(s => s.Theatre)
            .FirstOrDefaultAsync(s => s.ScreenId == id);
    }

    public async Task<List<Screen>> GetByTheatreIdAsync(int theatreId)
    {
        return await _context.Screens
            .Include(s => s.Theatre)
            .Where(s => s.TheatreId == theatreId)
            .ToListAsync();
    }

    public async Task<Screen> CreateAsync(Screen screen)
    {
        _context.Screens.Add(screen);

        await _context.SaveChangesAsync();

        return await GetByIdAsync(screen.ScreenId) ?? screen;
    }

    public async Task<Screen?> UpdateAsync(int id, Screen screen)
    {
        var existingScreen = await _context.Screens.FindAsync(id);

        if (existingScreen == null)
        {
            return null;
        }

        existingScreen.TheatreId = screen.TheatreId;
        existingScreen.ScreenName = screen.ScreenName;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var screen = await _context.Screens.FindAsync(id);

        if (screen == null)
        {
            return false;
        }

        _context.Screens.Remove(screen);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> TheatreExistsAsync(int theatreId)
    {
        return await _context.Theatres
            .AnyAsync(t => t.TheatreId == theatreId);
    }
}
