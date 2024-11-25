using InfoTrack.Api.Controllers;
using InfoTrack.Contracts.Interfaces;
using InfoTrack.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using InfoTrack.Contracts.Models;
using InfoTrack.Contracts.ResultPattern;

namespace InfoTrack.Tests.Api.Controllers;

public class SettlementControllerTests
{
    private readonly Mock<ISettlementService> _settlementServiceMock;
    private readonly SettlementController _sut;
    private readonly Mock<ILogger<SettlementController>> _loggerMock;

    public SettlementControllerTests()
    {
        _settlementServiceMock = new Mock<ISettlementService>();
        _loggerMock = new Mock<ILogger<SettlementController>>();
        _sut = new SettlementController(_loggerMock.Object, _settlementServiceMock.Object);
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

        var bookingResponse = new BookingResponse(Guid.NewGuid());

        _settlementServiceMock.Setup(s => s.BookSettlementAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(bookingResponse);

        // Act
        var result = await _sut.BookSettlementAsync(bookingRequest);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeEquivalentTo(bookingResponse);        
    }

    [Fact]
    public async Task When_InvalidModelState_Then_BookSettlementAsync_Should_ReturnsBadRequest()
    {
        // Arrange        
        _sut.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _sut.BookSettlementAsync(new BookingRequest());

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
        var bookingRequest = new BookingRequest
        {
            BookingTime = "10:00",
            Name = "Jane Doe"
        };

        _settlementServiceMock.Setup(s => s.BookSettlementAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(SettlementErrors.Conflict(bookingRequest.BookingTime));
       
        // Act
        var result = await _sut.BookSettlementAsync(bookingRequest);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var conflictResult = (ObjectResult)result;
        var problemDetail = (ProblemDetails)conflictResult.Value;

        problemDetail.Title.Should().BeEquivalentTo($"No slot available for booking time: {bookingRequest.BookingTime}.");
    }
}
