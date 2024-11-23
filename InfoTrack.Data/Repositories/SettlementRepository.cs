using InfoTrack.Contracts.Interfaces;
using InfoTrack.Contracts.Models;
using Microsoft.Extensions.Logging;

namespace InfoTrack.Data.Repositories;

public class SettlementRepository : ISettlementRepository
{
    private static readonly List<Booking> _bookings = [];
    private readonly ILogger<SettlementRepository> _logger;

    public SettlementRepository(ILogger<SettlementRepository> logger)
    {
        _logger = logger;
    }

    public async Task<bool> IsSlotAvailableAsync(DateTime bookingTime)
    {
        _logger.LogInformation("Checking slot availability for booking time: {BookingTime}", bookingTime);
        // Simulate asynchronous query operation in database for demonstration
        await Task.Delay(100); 
        
        // Check for existing bookings within the hour
        return _bookings.Count(b =>
            b.BookingTime >= bookingTime &&
            b.BookingTime < bookingTime.AddHours(1)
        ) < 4;
    }

    public async Task<Guid> AddBookingAsync(DateTime bookingTime, string name)
    {
        _logger.LogInformation("Adding booking for time: {BookingTime}, name: {Name}", bookingTime, name);
        // Simulate asynchronous adding new booking operation in database for demonstration
        await Task.Delay(100);
        var booking = new Booking(name, bookingTime);
        _bookings.Add(booking);
        return booking.Id;
    }
}
