namespace MovieTicketBooking.DTOs.Bookings;

public class CancelBookingResponse
{
    public int BookingId { get; set; }

    public string BookingReference { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}