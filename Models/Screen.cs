namespace MovieTicketBooking.Models;

public class Screen
{
    public int ScreenId { get; set; }

    public int TheatreId { get; set; }

    public string ScreenName { get; set; } = string.Empty;

    public Theatre Theatre { get; set; } = null!;

    // One Screen can have multiple Seats
    public ICollection<Seat> Seats { get; set; } = new List<Seat>();

    // One Screen can host multiple Shows
    public ICollection<Show> Shows { get; set; } = new List<Show>();
}