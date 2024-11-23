namespace InfoTrack.Contracts.Models;

public class Booking(string name, DateTime? bookingTime)
{
    public Guid Id { get; } = Guid.NewGuid();
    public string? Name { get; private set; } = name;
    public DateTime? BookingTime { get; private set; } = bookingTime;
}
