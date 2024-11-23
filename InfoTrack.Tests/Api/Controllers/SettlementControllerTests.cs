using InfoTrack.Api.Controllers;
using InfoTrack.Contracts.Interfaces;
using InfoTrack.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace InfoTrack.Tests.Api.Controllers;

public class SettlementControllerTests
{
    private readonly Mock<ISettlementService> _settlementServiceMock;
    private readonly SettlementController _controller;
    private readonly Mock<ILogger<SettlementController>> _loggerMock;

    public SettlementControllerTests()
    {
        _settlementServiceMock = new Mock<ISettlementService>();
        _loggerMock = new Mock<ILogger<SettlementController>>();
        _controller = new SettlementController(_loggerMock.Object, _settlementServiceMock.Object);
    }

    [Fact]
    public async Task When_ValidRequest_Then_BookSettlementAsync_Should_ReturnsOkWithBookingId()
    {
        // Arrange
        var bookingRequest = new BookingRequest
        {
            BookingTime = "10:00",
            Name = "John Doe"
        };

        var _bookingId = Guid.NewGuid();
        _settlementServiceMock.Setup(s => s.BookSettlementAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_bookingId);

        // Act
        var result = await _controller.BookSettlementAsync(bookingRequest);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeEquivalentTo(new { bookingId = _bookingId });        
    }

    [Fact]
    public async Task When_InvalidModelState_Then_BookSettlementAsync_Should_ReturnsBadRequest()
    {
        // Arrange        
        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.BookSettlementAsync(new BookingRequest());

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result;        
        badRequestResult.Value.Should().BeOfType<SerializableError>();
        var errors = (SerializableError)badRequestResult.Value;
        errors.Should().ContainKey("Name");        
    }

    [Fact]
    public async Task When_NoAvailableSlots_Then_BookSettlementAsync_Should_ReturnsConflict()
    {
        // Arrange
        _settlementServiceMock.Setup(s => s.BookSettlementAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((Guid?)null);
        
        var bookingRequest = new BookingRequest
        {
            BookingTime = "10:00",
            Name = "Jane Doe"
        };

        // Act
        var result = await _controller.BookSettlementAsync(bookingRequest);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
        var conflictResult = (ConflictObjectResult)result;
        conflictResult.Value.Should().BeEquivalentTo($"No available slots at {bookingRequest.BookingTime}.");
    }
}
