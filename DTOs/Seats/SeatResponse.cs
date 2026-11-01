namespace MovieTicketBooking.DTOs.Seats;

public class SeatResponse
{
    public int SeatId { get; set; }

    public int ScreenId { get; set; }

    public string ScreenName { get; set; } = string.Empty;

    public string RowName { get; set; } = string.Empty;

    public int SeatNumber { get; set; }

    public string SeatType { get; set; } = string.Empty;
}