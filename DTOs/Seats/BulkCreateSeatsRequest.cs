using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Seats;

public class BulkCreateSeatsRequest
{
    [Range(1, int.MaxValue)]
    public int ScreenId { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreateSeatItemRequest> Seats { get; set; } = new();
}

public class CreateSeatItemRequest
{
    [Required]
    [StringLength(5)]
    public string RowName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int SeatNumber { get; set; }

    [Required]
    [StringLength(30)]
    public string SeatType { get; set; } = string.Empty;
}