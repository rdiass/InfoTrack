using InfoTrack.Data.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace InfoTrack.Tests.Data.Repositories;

public class SettlementRepositoryTests
{
    private readonly Mock<ILogger<SettlementRepository>> _mockLogger;
    private readonly SettlementRepository _sut;

    public SettlementRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<SettlementRepository>>();
        _sut = new SettlementRepository(_mockLogger.Object);
    }

    [Fact]
    public async Task When_SlotAvailable_Then_GetSlotsOccupiedInDateTimeRangeAsync_Should_ReturnLessThenFour()
    {
        // Arrange        
        var bookingTime = DateTime.UtcNow;

        // Act
        var isAvailable = await _sut.GetSlotsOccupiedInDateTimeRangeAsync(bookingTime, bookingTime.AddHours(1));

        // Assert
        isAvailable.Should().BeLessThan(4);
    }

    [Fact]
    public async Task When_ValidData_Then_AddBookingAsync_Should_ReturnId()
    {
        // Arrange
        var bookingTime = DateTime.UtcNow;
        var name = "John Doe";

        // Act
        var bookingId = await _sut.AddBookingAsync(bookingTime, bookingTime.AddHours(1), name);

        // Assert
        bookingId.Should().NotBeEmpty();
    }
}
