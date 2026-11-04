using System.Data;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Admin/Manager: all bookings
    // Customer: only their bookings
    public async Task<List<Booking>> GetAllAsync(int? customerId)
    {
        var query = _context.Bookings
            .Include(b => b.BookingSeats)
            .AsQueryable();

        if (customerId.HasValue)
        {
            query = query.Where(
                b => b.CustomerId == customerId.Value);
        }

        return await query
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    // Admin/Manager: any booking
    // Customer: only their booking
    public async Task<Booking?> GetByIdAsync(
        int bookingId,
        int? customerId)
    {
        var query = _context.Bookings
            .Include(b => b.BookingSeats)
            .Where(b => b.BookingId == bookingId);

        if (customerId.HasValue)
        {
            query = query.Where(
                b => b.CustomerId == customerId.Value);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<Booking?> GetByReferenceAsync(
        string bookingReference)
    {
        return await _context.Bookings
            .Include(b => b.BookingSeats)
            .FirstOrDefaultAsync(
                b => b.BookingReference == bookingReference);
    }

    public async Task<Booking> CreateAsync(
        Booking booking,
        List<int> seatIds)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var show = await _context.Shows
            .FirstOrDefaultAsync(s => s.ShowId == booking.ShowId);

        if (show == null)
        {
            throw new InvalidOperationException("Show not found.");
        }

        var showSeats = await _context.ShowSeats
            .Where(ss =>
                ss.ShowId == booking.ShowId &&
                seatIds.Contains(ss.SeatId))
            .ToListAsync();

        if (showSeats.Count != seatIds.Count)
        {
            throw new InvalidOperationException(
                "One or more selected seats do not belong to this show.");
        }

        if (showSeats.Any(ss => ss.Status != "Available"))
        {
            throw new InvalidOperationException(
                "One or more selected seats are already booked.");
        }

        booking.TotalAmount = show.TicketPrice * seatIds.Count;

        booking.BookingSeats = seatIds.Select(seatId =>
            new BookingSeat
            {
                SeatId = seatId,
                TicketPrice = show.TicketPrice
            }).ToList();

        foreach (var showSeat in showSeats)
        {
            showSeat.Status = "Booked";
        }

        _context.Bookings.Add(booking);

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return booking;
    }

    // Admin/Manager: any booking
    // Customer: only their booking
    public async Task<Booking?> CancelAsync(
        int bookingId,
        int? customerId)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var query = _context.Bookings
            .Include(b => b.BookingSeats)
            .Where(b => b.BookingId == bookingId);

        if (customerId.HasValue)
        {
            query = query.Where(
                b => b.CustomerId == customerId.Value);
        }

        var booking = await query.FirstOrDefaultAsync();

        if (booking == null)
        {
            await transaction.RollbackAsync();
            return null;
        }

        if (booking.Status != "Confirmed")
        {
            throw new InvalidOperationException(
                "Only confirmed bookings can be cancelled.");
        }

        var seatIds = booking.BookingSeats
            .Select(bs => bs.SeatId)
            .ToList();

        var showSeats = await _context.ShowSeats
            .Where(ss =>
                ss.ShowId == booking.ShowId &&
                seatIds.Contains(ss.SeatId))
            .ToListAsync();

        foreach (var showSeat in showSeats)
        {
            showSeat.Status = "Available";
        }

        booking.Status = "Cancelled";

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return booking;
    }
}
