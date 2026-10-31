namespace MovieTicketBooking.Models;

public class Theatre
{
    // Primary Key
    public int TheatreId { get; set; }

    // Theatre Name
    public string Name { get; set; } = string.Empty;

    // Theatre Location
    public string Location { get; set; } = string.Empty;
}