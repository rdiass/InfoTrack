﻿using InfoTrack.Contracts.Interfaces;
using Microsoft.Extensions.Logging;

namespace InfoTrack.Business.Services
{

    public class SettlementService : ISettlementService
    {
        private readonly ISettlementRepository _settlementRepository;
        private readonly ILogger<SettlementService> _logger;
        private static readonly SemaphoreSlim semaphoreSlim = new(1, 1);

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
            // Asynchronously wait to enter the Semaphore. If no-one has been granted access to the Semaphore, code execution will proceed, otherwise this thread waits here until the semaphore is released 
            // This is not the right solution but will lock the booking avoiding the overbooking 
            // For this solution I recommend:
            // 1 - Distributed Locking Services (Redis, Azure Distributed Lock)
            // 2 - Message Queues (Azure service bus)
            // 3 - Database-Based Locking
            // Example:
            // execute - await _redisCache.StringSetAsync
            // in finally - await _redisCache.KeyDeleteAsync
            await semaphoreSlim.WaitAsync();
            try
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
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                semaphoreSlim.Release();
            }
        }
    }
}
