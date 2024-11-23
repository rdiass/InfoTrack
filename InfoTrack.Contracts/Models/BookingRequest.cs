using InfoTrack.Contracts.Extensions;
using System.ComponentModel.DataAnnotations;

namespace InfoTrack.Contracts.Requests;

/// <summary>
/// Booking request with data notation to validate data
/// </summary>
public class BookingRequest
{
    [Required(ErrorMessage = "Booking time is required")]
    [RegularExpression(@"^\d{2}:\d{2}$", ErrorMessage = "Invalid time format. Please use HH:mm")]
    [ValidBookingTimeAttribute]
    public string? BookingTime { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MinLength(1, ErrorMessage = "Name cannot be empty")]
    public string? Name { get; set; }
}