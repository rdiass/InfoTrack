namespace InfoTrack.Contracts.Interfaces;

public interface ISettlementRepository
{
    Task<bool> IsSlotAvailableAsync(DateTime bookingTime, DateTime bookingEndTime);
    Task<Guid> AddBookingAsync(DateTime bookingTime, DateTime bookingEndTime, string name);
}
