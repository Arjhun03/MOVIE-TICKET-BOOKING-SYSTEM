
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data;

public class DbSeeder(
    ApplicationDbContext context,
    IPasswordHasher<User> passwordHasher,
    IConfiguration configuration)
{
    public async Task SeedAsync()
    {
        await context.Database.MigrateAsync();

        await SeedUserAsync(
            "Admin",
            "SeedUsers:AdminEmail",
            "SeedUsers:AdminPassword");

        await SeedUserAsync(
            "Manager",
            "SeedUsers:ManagerEmail",
            "SeedUsers:ManagerPassword");

        await SeedTheatreManagerAsync();
    }

    private async Task SeedUserAsync(
        string role,
        string emailKey,
        string passwordKey)
    {
        var email = configuration[emailKey]?.Trim().ToLowerInvariant();
        var password = configuration[passwordKey];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                $"Missing configuration for {role} seed user.");
        }

        var existingUser = await context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (existingUser != null)
            return;

        var user = new User
        {
            FullName = role,
            Email = email,
            Role = role
        };

        user.PasswordHash = passwordHasher.HashPassword(user, password);

        context.Users.Add(user);
        await context.SaveChangesAsync();
    }

    private async Task SeedTheatreManagerAsync()
    {
        var name = configuration["SeedUsers:TheatreManagerName"];
        var email = configuration["SeedUsers:TheatreManagerEmail"]
            ?.Trim().ToLowerInvariant();
        var password = configuration["SeedUsers:TheatreManagerPassword"];
        var theatreIdValue = configuration["SeedUsers:TheatreManagerTheatreId"];

        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            !int.TryParse(theatreIdValue, out var theatreId))
        {
            throw new InvalidOperationException(
                "Invalid Theatre Manager configuration.");
        }

        var theatreExists = await context.Theatres
            .AnyAsync(t => t.TheatreId == theatreId);

        if (!theatreExists)
        {
            throw new InvalidOperationException(
                $"Theatre with ID {theatreId} does not exist.");
        }

        var existingUser = await context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (existingUser != null)
        {
            if (existingUser.Role != "Manager")
            {
                throw new InvalidOperationException(
                    "The Theatre Manager email belongs to another role.");
            }

            existingUser.TheatreId = theatreId;
            await context.SaveChangesAsync();
            return;
        }

        var manager = new User
        {
            FullName = name,
            Email = email,
            Role = "Manager",
            TheatreId = theatreId
        };

        manager.PasswordHash = passwordHasher.HashPassword(
            manager,
            password);

        context.Users.Add(manager);
        await context.SaveChangesAsync();
    }

    public async Task ResetAdminPasswordAsync()
    {
        await ResetPasswordAsync(
            "SeedUsers:AdminEmail",
            "SeedUsers:AdminPassword",
            "Admin");
    }

    public async Task ResetManagerPasswordAsync()
    {
        await ResetPasswordAsync(
            "SeedUsers:ManagerEmail",
            "SeedUsers:ManagerPassword",
            "Manager");
    }

    private async Task ResetPasswordAsync(
        string emailKey,
        string passwordKey,
        string role)
    {
        var email = configuration[emailKey]?.Trim().ToLowerInvariant();
        var password = configuration[passwordKey];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                $"Missing configuration for {role} password reset.");
        }

        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            throw new InvalidOperationException(
                $"{role} account not found.");
        }

        user.PasswordHash = passwordHasher.HashPassword(user, password);

        await context.SaveChangesAsync();
    }
}
