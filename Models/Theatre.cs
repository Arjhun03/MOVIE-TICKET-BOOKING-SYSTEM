namespace MovieTicketBooking.Models;

public class Theatre
{
    public int TheatreId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    // One Theatre can have multiple Screens
    public ICollection<Screen> Screens { get; set; } = new List<Screen>();

    // One Theatre can have multiple Managers
    public ICollection<User> Managers { get; set; } = new List<User>();
}