namespace MovieTicketBooking.Models;

public class Screen
{
    public int ScreenId { get; set; }

    public int TheatreId { get; set; }

    public string ScreenName { get; set; } = string.Empty;

    public Theatre Theatre { get; set; } = null!;
}