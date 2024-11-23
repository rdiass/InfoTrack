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
    private readonly SettlementService _settlementService;

    public SettlementServiceTests()
    {
        _mockSettlementRepository = new Mock<ISettlementRepository>();
        _mockLogger = new Mock<ILogger<SettlementService>>();
        _settlementService = new SettlementService(_mockSettlementRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task When_SlotAvailable_Then_BookSettlementAsync_Should_ReturnBookingId()
    {
        // Arrange
        var bookingTime = "10:00";
        var expectedBookingId = Guid.NewGuid();
        _mockSettlementRepository.Setup(r => r.IsSlotAvailableAsync(It.IsAny<DateTime>()))
            .Returns(Task.FromResult(true));
        _mockSettlementRepository.Setup(r => r.AddBookingAsync(It.IsAny<DateTime>(), It.IsAny<string>()))
            .Returns(Task.FromResult(expectedBookingId));

        // Act
        var bookingId = await _settlementService.BookSettlementAsync(bookingTime, "John Doe");

        // Assert
        bookingId.Should().Be(expectedBookingId);
    }

    [Fact]
    public async Task When_SlotNotAvailable_Then_BookSettlementAsync_ShouldReturnNull()
    {
        // Arrange
        var bookingTime = "10:00";
        _mockSettlementRepository.Setup(r => r.IsSlotAvailableAsync(It.IsAny<DateTime>()))
            .Returns(Task.FromResult(false));

        // Act
        var bookingId = await _settlementService.BookSettlementAsync(bookingTime, "John Doe");

        // Assert
        bookingId.Should().BeNull();
        _mockSettlementRepository.Verify(r => r.AddBookingAsync(It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void When_BookingTimeIsNull_Then_BookSettlementAsync_Should_ThrowArgumentNullException()
    {
        // Arrange
        string bookingTime = null;
        string name = "John Doe";

        // Act & Assert
        Func<Task<Guid?>> act = async () => await _settlementService.BookSettlementAsync(bookingTime, name);
        act.Should().ThrowAsync<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'bookingTime')");
    }

    [Fact]
    public void When_NameIsNull_Then_BookSettlementAsync_Should_ThrowArgumentNullException()
    {
        // Arrange
        string bookingTime = "10:00";
        string name = null;

        // Act & Assert
        Func<Task<Guid?>> act = async () => await _settlementService.BookSettlementAsync(bookingTime, null);
        act.Should().ThrowAsync<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'name')");
    }
}
