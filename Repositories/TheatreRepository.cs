using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class TheatreRepository : ITheatreRepository
{
    private readonly ApplicationDbContext _context;

    public TheatreRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Theatre>> GetAllAsync()
    {
        return await _context.Theatres.ToListAsync();
    }

    public async Task<Theatre?> GetByIdAsync(int id)
    {
        return await _context.Theatres.FindAsync(id);
    }

    public async Task<Theatre> CreateAsync(Theatre theatre)
    {
        _context.Theatres.Add(theatre);

        await _context.SaveChangesAsync();

        return theatre;
    }

    public async Task<Theatre?> UpdateAsync(int id, Theatre theatre)
    {
        var existingTheatre = await _context.Theatres.FindAsync(id);

        if (existingTheatre == null)
        {
            return null;
        }

        existingTheatre.Name = theatre.Name;
        existingTheatre.Location = theatre.Location;

        await _context.SaveChangesAsync();

        return existingTheatre;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var theatre = await _context.Theatres.FindAsync(id);

        if (theatre == null)
        {
            return false;
        }

        _context.Theatres.Remove(theatre);

        await _context.SaveChangesAsync();

        return true;
    }
}