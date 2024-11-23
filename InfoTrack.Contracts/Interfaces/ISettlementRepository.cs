namespace InfoTrack.Contracts.Interfaces;

public interface ISettlementRepository
{
    Task<bool> IsSlotAvailableAsync(DateTime bookingTime);
    Task<Guid> AddBookingAsync(DateTime bookingTime, string name);
}
