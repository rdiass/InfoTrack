using InfoTrack.Contracts.Requests;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using InfoTrack.Api.Controllers;
using InfoTrack.Contracts.Models;
using Microsoft.AspNetCore.Mvc;

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
    public async Task When_DateIsNotInBusinessHour_Then_BookSettlementAsync_Should_ReturnsBadRequest()
    {
        // Arrange
        var bookingRequest = new BookingRequest
        {
            Name = "John Doe",
            BookingTime = "16:30"
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
            BookingTime = "09:29"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/settlement/book", bookingRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var content = await response.Content.ReadAsStringAsync();
        var problemDetail = JsonConvert.DeserializeObject<ProblemDetails>(content);

        problemDetail.Title.Should().BeEquivalentTo($"No slot available for booking time: {bookingRequest.BookingTime}.");
    }

    [Fact]
    public async Task When_OneSlotIsAvailable_Then_BookSettlementAsync_Should_ReturnSuccess()
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
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var booking = JsonConvert.DeserializeObject<BookingResponse>(content);
        booking.Should().NotBeNull();
        booking?.BookingId.Should().NotBeEmpty();
    }

    private async Task FillSlotsWithBookingTime()
    {
        string[] times = ["09:30", "09:45", "09:50", "10:00"];
        var tasks = new List<Task>();
        var bookingRequest = new BookingRequest
        {
            Name = "John Doe"
        };

        foreach (var time in times)
        {
            bookingRequest.BookingTime = time;
            tasks.Add(_client.PostAsJsonAsync("/api/settlement/book", bookingRequest));
        }

        await Task.WhenAll(tasks);
    }
}
