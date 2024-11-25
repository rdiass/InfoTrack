using FluentAssertions;
using InfoTrack.Business.Services;
using InfoTrack.Contracts.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace InfoTrack.Tests.Business.Services;

public class SettlementServiceTests
{
    private readonly Mock<ISettlementRepository> _mockSettlementRepository;
    private readonly Mock<ILogger<SettlementService>> _mockLogger;
    private readonly SettlementService _sut;

    public SettlementServiceTests()
    {
        _mockSettlementRepository = new Mock<ISettlementRepository>();
        _mockLogger = new Mock<ILogger<SettlementService>>();
        _sut = new SettlementService(_mockSettlementRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task When_SlotAvailable_Then_BookSettlementAsync_Should_ReturnBookingId()
    {
        // Arrange
        var bookingTime = "10:00";
        var expectedBookingId = Guid.NewGuid();
        _mockSettlementRepository.Setup(r => r.GetSlotsOccupiedInDateTimeRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(Task.FromResult(3));
        _mockSettlementRepository.Setup(r => r.AddBookingAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
            .Returns(Task.FromResult(expectedBookingId));

        // Act
        var result = await _sut.BookSettlementAsync(bookingTime, "John Doe");

        // Assert
        result.Value.BookingId.Should().Be(expectedBookingId);
    }

    [Fact]
    public async Task When_SlotNotAvailable_Then_BookSettlementAsync_ShouldReturnError()
    {
        // Arrange
        var bookingTime = "10:00";
        _mockSettlementRepository.Setup(r => r.GetSlotsOccupiedInDateTimeRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(Task.FromResult(4));

        // Act
        var result = await _sut.BookSettlementAsync(bookingTime, "John Doe");

        // Assert
        result.Error.Should().NotBeNull();
        _mockSettlementRepository.Verify(r => r.AddBookingAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
    }
}
