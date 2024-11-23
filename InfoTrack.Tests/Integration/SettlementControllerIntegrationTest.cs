using InfoTrack.Contracts.Requests;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using InfoTrack.Api.Controllers;
using InfoTrack.Contracts.Models;

namespace InfoTrack.Tests.Integration;

public class SettlementControllerIntegrationTest
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<SettlementController> _factory;

    public SettlementControllerIntegrationTest()
    {
        _factory = new WebApplicationFactory<SettlementController>();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task When_ValidDate_Then_BookSettlementAsync_Should_ReturnSuccess()
    {
        // Arrange
        var bookingRequest = new BookingRequest
        {
            Name = "John Doe",
            BookingTime = "10:30"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/settlement/book", bookingRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var booking = JsonConvert.DeserializeObject<BookingResponse>(content);
        booking.Should().NotBeNull();
        booking?.BookingId.Should().NotBeNull();

        var isGuid = Guid.TryParse(booking?.BookingId, out var guid);
        isGuid.Should().BeTrue();
        guid.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("InvalidTime", "John Doe")]
    [InlineData("", "John Doe")]
    [InlineData("12:00", "")]
    [InlineData("16:30", "John Doe")]
    public async Task When_InvalidData_Then_BookSettlementAsync_Should_ReturnsBadRequest(string bookingTime, string name)
    {
        // Arrange
        var bookingRequest = new BookingRequest
        {
            Name = name,
            BookingTime = bookingTime
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/settlement/book", bookingRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task When_SlotNotAvailable_Then_BookSettlementAsync_Should_ReturnConflict()
    {
        // Arrange
        await FillSlotsWithBookingTime();
        var bookingRequest = new BookingRequest
        {
            Name = "John Doe",
            BookingTime = "09:00"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/settlement/book", bookingRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().BeEquivalentTo($"No available slots at {bookingRequest.BookingTime}.");
    }

    private async Task FillSlotsWithBookingTime()
    {
        string[] times = ["09:00", "09:00", "09:00", "09:00"];

        foreach (var time in times)
        {
            // Arrange
            var bookingRequest = new BookingRequest
            {
                Name = "John Doe",
                BookingTime = time
            };

            await _client.PostAsJsonAsync("/api/settlement/book", bookingRequest);
        }
    }
}
