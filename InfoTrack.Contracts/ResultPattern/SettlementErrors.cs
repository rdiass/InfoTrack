namespace InfoTrack.Contracts.ResultPattern;

public static class SettlementErrors
{
    public static Error Conflict(string bookingTime) =>
        Error.Conflict("Settlement.Conflict", $"No slot available for booking time: {bookingTime}.");
}
