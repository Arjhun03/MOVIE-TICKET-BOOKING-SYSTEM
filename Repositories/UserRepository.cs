using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class UserRepository(ApplicationDbContext db) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email)
    {
        return db.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public Task AddAsync(User user)
    {
        return db.Users.AddAsync(user).AsTask();
    }

    public Task SaveChangesAsync()
    {
        return db.SaveChangesAsync();
    }
}