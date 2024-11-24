using InfoTrack.Contracts.Models;
using InfoTrack.Contracts.ResultPattern;

namespace InfoTrack.Contracts.Interfaces;

public interface ISettlementService
{
    Task<Result<BookingResponse>> BookSettlementAsync(string bookingTime, string name);
}
