namespace InfoTrack.Contracts.Models;

public class BookingResponse
{
    public BookingResponse(Guid bookingId) => BookingId = bookingId;
    public Guid BookingId { get; }
}
