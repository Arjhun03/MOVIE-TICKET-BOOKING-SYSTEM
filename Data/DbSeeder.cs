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
        {
            return;
        }

        var user = new User
        {
            FullName = role,
            Email = email,
            Role = role
        };

        user.PasswordHash = passwordHasher.HashPassword(
            user,
            password);

        context.Users.Add(user);

        await context.SaveChangesAsync();
    }
    
    public async Task ResetAdminPasswordAsync()
    {
        var email = configuration["SeedUsers:AdminEmail"]!
            .Trim()
            .ToLowerInvariant();

        var password = configuration["SeedUsers:AdminPassword"]!;

        var admin = await context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (admin == null)
        {
            throw new InvalidOperationException("Admin account not found.");
        }

        admin.PasswordHash = passwordHasher.HashPassword(admin, password);

        await context.SaveChangesAsync();
    }
}