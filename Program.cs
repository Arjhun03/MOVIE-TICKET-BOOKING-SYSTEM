using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Repositories;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services;
using MovieTicketBooking.Services.Interfaces;
using Scalar.AspNetCore;
using MovieTicketBooking.Repositories;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services;
using MovieTicketBooking.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Register Controllers
builder.Services.AddControllers();

// Database Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Theatre
builder.Services.AddScoped<ITheatreRepository, TheatreRepository>();
builder.Services.AddScoped<ITheatreService, TheatreService>();

// Movie
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();

// Screen
builder.Services.AddScoped<IScreenRepository, ScreenRepository>();
builder.Services.AddScoped<IScreenService, ScreenService>();

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// Scalar
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();