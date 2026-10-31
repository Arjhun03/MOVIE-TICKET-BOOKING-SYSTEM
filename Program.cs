using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Repositories;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services;
using MovieTicketBooking.Services.Interfaces;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Register Controllers
builder.Services.AddControllers();

// Database Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Theatre Repository and Service
builder.Services.AddScoped<ITheatreRepository, TheatreRepository>();
builder.Services.AddScoped<ITheatreService, TheatreService>();

// Movie Repository and Service
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();

// Register OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure Scalar
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();