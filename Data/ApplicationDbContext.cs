
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Theatre> Theatres { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Screen> Screens { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<Show> Shows { get; set; }
    public DbSet<ShowSeat> ShowSeats { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingSeat> BookingSeats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Theatre Configuration
        modelBuilder.Entity<Theatre>(entity =>
        {
            entity.ToTable("Theatres");

            entity.HasKey(t => t.TheatreId);

            entity.Property(t => t.TheatreId)
                .ValueGeneratedOnAdd();

            entity.Property(t => t.Name)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(t => t.Location)
                .HasMaxLength(200)
                .IsRequired();
        });

        // Movie Configuration
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.ToTable("Movies");

            entity.HasKey(m => m.MovieId);

            entity.Property(m => m.MovieId)
                .ValueGeneratedOnAdd();

            entity.Property(m => m.Title)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(m => m.Description)
                .HasMaxLength(500);

            entity.Property(m => m.DurationMinutes)
                .IsRequired();

            entity.Property(m => m.ReleaseDate)
                .HasColumnType("date")
                .IsRequired();

            entity.Property(m => m.Language)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(m => m.Genre)
                .HasMaxLength(50)
                .IsRequired();
        });

        // Screen Configuration
        modelBuilder.Entity<Screen>(entity =>
        {
            entity.ToTable("Screens");

            entity.HasKey(s => s.ScreenId);

            entity.Property(s => s.ScreenId)
                .ValueGeneratedOnAdd();

            entity.Property(s => s.ScreenName)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasOne(s => s.Theatre)
                .WithMany(t => t.Screens)
                .HasForeignKey(s => s.TheatreId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(s => new { s.TheatreId, s.ScreenName })
                .IsUnique();

            entity.HasMany(s => s.Seats)
                .WithOne(seat => seat.Screen)
                .HasForeignKey(seat => seat.ScreenId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seat Configuration
        modelBuilder.Entity<Seat>(entity =>
        {
            entity.ToTable("Seats");

            entity.HasKey(s => s.SeatId);

            entity.Property(s => s.SeatId)
                .ValueGeneratedOnAdd();

            entity.Property(s => s.RowName)
                .HasMaxLength(5)
                .IsRequired();

            entity.Property(s => s.SeatNumber)
                .IsRequired();

            entity.Property(s => s.SeatType)
                .HasMaxLength(30)
                .IsRequired();

            entity.HasIndex(s => new
            {
                s.ScreenId,
                s.RowName,
                s.SeatNumber
            }).IsUnique();
        });

        // Show Configuration
        modelBuilder.Entity<Show>(entity =>
        {
            entity.ToTable("Shows", table =>
            {
                table.HasCheckConstraint(
                    "CK_Shows_EndTime_After_StartTime",
                    "[EndTime] > [StartTime]");

                table.HasCheckConstraint(
                    "CK_Shows_TicketPrice_NonNegative",
                    "[TicketPrice] >= 0");
            });

            entity.HasKey(s => s.ShowId);

            entity.Property(s => s.ShowId)
                .ValueGeneratedOnAdd();

            entity.Property(s => s.StartTime)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(s => s.EndTime)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(s => s.TicketPrice)
                .HasPrecision(10, 2)
                .IsRequired();

            entity.HasOne(s => s.Movie)
                .WithMany(m => m.Shows)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.Screen)
                .WithMany(s => s.Shows)
                .HasForeignKey(s => s.ScreenId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ShowSeat Configuration
        modelBuilder.Entity<ShowSeat>(entity =>
        {
            entity.ToTable("ShowSeats", table =>
            {
                table.HasCheckConstraint(
                    "CK_ShowSeats_Status",
                    "[Status] IN ('Available', 'Booked')");
            });

            entity.HasKey(ss => ss.ShowSeatId);

            entity.Property(ss => ss.ShowSeatId)
                .ValueGeneratedOnAdd();

            entity.Property(ss => ss.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Available");

            entity.HasOne(ss => ss.Show)
                .WithMany(s => s.ShowSeats)
                .HasForeignKey(ss => ss.ShowId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ss => ss.Seat)
                .WithMany(s => s.ShowSeats)
                .HasForeignKey(ss => ss.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(ss => new { ss.ShowId, ss.SeatId })
                .IsUnique();
        });

        // Booking Configuration
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("Bookings", table =>
            {
                table.HasCheckConstraint(
                    "CK_Bookings_Status",
                    "[Status] IN ('Confirmed', 'Cancelled')");
            });

            entity.HasKey(b => b.BookingId);

            entity.Property(b => b.BookingId)
                .ValueGeneratedOnAdd();

            entity.Property(b => b.BookingReference)
                .HasMaxLength(30)
                .IsRequired();

            entity.HasIndex(b => b.BookingReference)
                .IsUnique();

            entity.Property(b => b.CustomerName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(b => b.CustomerEmail)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(b => b.CustomerPhone)
                .HasMaxLength(15)
                .IsRequired();

            entity.Property(b => b.BookingDate)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(b => b.TotalAmount)
                .HasPrecision(10, 2)
                .IsRequired();

            entity.Property(b => b.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Confirmed")
                .IsRequired();

            entity.HasOne(b => b.Show)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ShowId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // BookingSeat Configuration
        modelBuilder.Entity<BookingSeat>(entity =>
        {
            entity.ToTable("BookingSeats");

            entity.HasKey(bs => bs.BookingSeatId);

            entity.Property(bs => bs.BookingSeatId)
                .ValueGeneratedOnAdd();

            entity.Property(bs => bs.TicketPrice)
                .HasPrecision(10, 2)
                .IsRequired();

            entity.HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingSeats)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(bs => bs.Seat)
                .WithMany(s => s.BookingSeats)
                .HasForeignKey(bs => bs.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(bs => new { bs.BookingId, bs.SeatId })
                .IsUnique();
        });
    }
}
