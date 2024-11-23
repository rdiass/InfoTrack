﻿using InfoTrack.Contracts.Requests;
using InfoTrack.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InfoTrack.Api.Controllers;

/// <summary>
/// Controller responsible for handling settlement-related operations.
/// This controller exposes endpoints for booking settlements and potentially other settlement-related actions.
/// </summary>
[ApiController]
[Route("api/settlement")]
public class SettlementController : ControllerBase
{
    private readonly ILogger<SettlementController> _logger;
    private readonly ISettlementService _settlementService;

    /// <summary>
    /// Initializes the controller with necessary dependencies
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="settlementService"></param>
    public SettlementController(ILogger<SettlementController> logger, ISettlementService settlementService)
    {
        _logger = logger;
        _settlementService = settlementService;
    }

    /// <summary>
    /// Books a settlement for a specific time and name.
    /// </summary>
    /// <param name="request">Booking request containing booking time and name.</param>
    /// <returns>IActionResult containing either booking id (success) 
    /// or a bad request/conflict message (failure).</returns>
    [HttpPost("book")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult>BookSettlementAsync([FromBody] BookingRequest request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogDebug("Booking request failed due to invalid model state");
            return BadRequest(ModelState);
        }

        var bookingId = await _settlementService.BookSettlementAsync(request.BookingTime, request.Name);

        if (bookingId == null)
        {
            _logger.LogInformation("Booking failed - no available slots at {BookingTime}", request.BookingTime);
            return Conflict($"No available slots at {request.BookingTime}.");
        }

        _logger.LogInformation("Booking successful - Booking ID: {BookingId}", bookingId);
        return Ok(new { bookingId });
    }

}