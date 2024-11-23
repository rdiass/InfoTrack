using InfoTrack.Contracts.Interfaces;
using Microsoft.Extensions.Logging;

namespace InfoTrack.Business.Services
{

    public class SettlementService : ISettlementService
    {
        private readonly ISettlementRepository _settlementRepository;
        private readonly ILogger<SettlementService> _logger;

        public SettlementService(ISettlementRepository settlementRepository, ILogger<SettlementService> logger)
        {
            _settlementRepository = settlementRepository;
            _logger = logger;
        }

        /// <summary>
        /// Attempts to book a settlement for the given name at the specified booking time.
        /// </summary>
        /// <param name="bookingTime">The desired time for the settlement.</param>
        /// <param name="name">The name of the party booking the settlement.</param>
        /// <returns>A GUID representing the settlement ID if successful, null otherwise.</returns>
        public async Task<Guid?> BookSettlementAsync(string bookingTime, string name)
        {
            _logger.LogInformation($"Checking slot availability for booking time: {{bookingTime}}", bookingTime);

            // Convert booking time to DateTime for validation
            var requestedDateTime = DateTime.Parse(bookingTime);

            if (await _settlementRepository.IsSlotAvailableAsync(requestedDateTime))
            {
                _logger.LogInformation($"Slot available for booking time: {{bookingTime}}", bookingTime);

                var bookingId = await _settlementRepository.AddBookingAsync(requestedDateTime, name);
                _logger.LogInformation($"Settlement booked successfully with ID: {{bookingId}}", bookingId);

                return bookingId;
            }

            _logger.LogWarning($"No slot available for booking time: {{bookingTime}}", bookingTime);

            return null;
        }
    }
}
