namespace InfoTrack.Contracts.Interfaces;

public interface ISettlementService
{
    Task<Guid?> BookSettlementAsync(string bookingTime, string name);
}
