namespace InfoTrack.Contracts.Models;

public class Booking
{
    public Booking(string name, DateTime? bookingTime, DateTime? bookingEndTime)
    {
        Name = name;
        BookingTime = bookingTime;
        BookingEndTime = bookingEndTime;
    }

    public Guid Id { get; } = Guid.NewGuid();
    public string? Name { get; set; }
    public DateTime? BookingTime { get; set; }
    public DateTime? BookingEndTime { get; set; }
}
