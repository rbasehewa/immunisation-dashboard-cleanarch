namespace Tests;

using Application.Interfaces;
using Application.Models;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Moq;

/// <summary>
/// Tests for DashboardService business logic.
/// Uses Moq to fake the repository — no database needed.
/// </summary>
public class DashboardServiceTests
{
    // Moq creates a FAKE IUserRepository
    private readonly Mock<IUserRepository> _mockRepo;
    private readonly DashboardService _service;

    public DashboardServiceTests()
    {
        // Create the fake repository
        _mockRepo = new Mock<IUserRepository>();

        // Inject the fake into the real service
        // Service doesn't know it's fake!
        _service = new DashboardService(_mockRepo.Object);
    }

    // ─────────────────────────────────────
    // STATISTICS TESTS
    // ─────────────────────────────────────

    [Fact]
    public async Task GetStatistics_ReturnsCorrectCompletionRate()
    {
        // ARRANGE: Tell the fake repo what to return
        _mockRepo
            .Setup(repo => repo.GetDashboardStatisticsAsync())
            .ReturnsAsync(new DashboardStatistics
            {
                TotalUsers = 10,
                FullyImmunised = 4,      // 4 out of 10
                PartiallyImmunised = 3,
                NonImmunised = 2,
                Overdue = 1
            });

        // ACT: Call the real service
        var result = await _service.GetDashboardStatisticsAsync();

        // ASSERT: Check completion rate = 4/10 * 100 = 40%
        Assert.Equal(10, result.TotalUsers);
        Assert.Equal(4, result.FullyImmunised);
        Assert.Equal(40.00m, result.ImmunisationCompletionRate);
    }

    [Fact]
    public async Task GetStatistics_ZeroUsers_ReturnsZeroRate()
    {
        // ARRANGE: Empty database scenario
        _mockRepo
            .Setup(repo => repo.GetDashboardStatisticsAsync())
            .ReturnsAsync(new DashboardStatistics
            {
                TotalUsers = 0,
                FullyImmunised = 0,
                PartiallyImmunised = 0,
                NonImmunised = 0,
                Overdue = 0
            });

        // ACT
        var result = await _service.GetDashboardStatisticsAsync();

        // ASSERT: Should not crash when dividing by zero!
        Assert.Equal(0, result.TotalUsers);
        Assert.Equal(0m, result.ImmunisationCompletionRate);
    }

    [Fact]
    public async Task GetStatistics_AllFullyImmunised_Returns100Percent()
    {
        // ARRANGE: Everyone is fully immunised
        _mockRepo
            .Setup(repo => repo.GetDashboardStatisticsAsync())
            .ReturnsAsync(new DashboardStatistics
            {
                TotalUsers = 5,
                FullyImmunised = 5,      // All 5!
                PartiallyImmunised = 0,
                NonImmunised = 0,
                Overdue = 0
            });

        // ACT
        var result = await _service.GetDashboardStatisticsAsync();

        // ASSERT: 5/5 * 100 = 100%
        Assert.Equal(100.00m, result.ImmunisationCompletionRate);
    }

    // ─────────────────────────────────────
    // USER SUMMARIES TESTS
    // ─────────────────────────────────────

    [Fact]
    public async Task GetUserSummaries_MapsUsersCorrectly()
    {
        // ARRANGE: Fake returns two users
        _mockRepo
            .Setup(repo => repo.GetAllUsersAsync())
            .ReturnsAsync(new List<User>
            {
                new User
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@example.com",
                    Status = ImmunisationStatus.FullyImmunised,
                    LastImmunisationDate = DateTime.UtcNow.AddMonths(-2) // Not overdue
                },
                new User
                {
                    Id = 4,
                    FirstName = "Alice",
                    LastName = "Williams",
                    Email = "alice@example.com",
                    Status = ImmunisationStatus.Overdue,
                    LastImmunisationDate = DateTime.UtcNow.AddYears(-2) // Overdue
                }
            });

        // ACT
        var result = await _service.GetUserSummariesAsync();
        var users = result.ToList();

        // ASSERT: Check mapping worked
        Assert.Equal(2, users.Count);

        // John checks
        Assert.Equal("John Doe", users[0].FullName);
        Assert.Equal("Fully Immunised", users[0].StatusDisplay);
        Assert.False(users[0].IsOverdue);
        Assert.True(users[0].IsFullyCompliant);

        // Alice checks
        Assert.Equal("Alice Williams", users[1].FullName);
        Assert.Equal("Overdue", users[1].StatusDisplay);
        Assert.True(users[1].IsOverdue);
        Assert.False(users[1].IsFullyCompliant);
    }

    // ─────────────────────────────────────
    // FILTER BY STATUS TESTS
    // ─────────────────────────────────────

    [Fact]
    public async Task GetUsersByStatus_InvalidStatus_ThrowsException()
    {
        // ACT & ASSERT: Invalid status should throw an error
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.GetUsersByStatusAsync("BananaStatus") // Not a real status!
        );
    }

    [Fact]
    public async Task GetUsersByStatus_ValidStatus_ReturnsFilteredUsers()
    {
        // ARRANGE: Fake returns only NonImmunised users
        _mockRepo
            .Setup(repo => repo.GetUsersByStatusAsync(ImmunisationStatus.NonImmunised))
            .ReturnsAsync(new List<User>
            {
                new User
                {
                    Id = 3,
                    FirstName = "Bob",
                    LastName = "Johnson",
                    Email = "bob@example.com",
                    Status = ImmunisationStatus.NonImmunised,
                    LastImmunisationDate = null
                }
            });

        // ACT
        var result = await _service.GetUsersByStatusAsync("NonImmunised");

        // ASSERT: Only Bob returned
        Assert.Single(result);
        Assert.Equal("Bob Johnson", result.First().FullName);
        Assert.Equal("Not Immunised", result.First().StatusDisplay);
    }

    [Fact]
    public async Task GetUsersByStatus_CaseInsensitive_Works()
    {
        // ARRANGE
        _mockRepo
            .Setup(repo => repo.GetUsersByStatusAsync(ImmunisationStatus.FullyImmunised))
            .ReturnsAsync(new List<User>
            {
                new User
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@example.com",
                    Status = ImmunisationStatus.FullyImmunised,
                    LastImmunisationDate = DateTime.UtcNow.AddMonths(-2)
                }
            });

        // ACT: lowercase "fullyimmunised" should still work
        var result = await _service.GetUsersByStatusAsync("fullyimmunised");

        // ASSERT
        Assert.Single(result);
        Assert.Equal("John Doe", result.First().FullName);
    }
}