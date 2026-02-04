namespace Tests;

using Domain.Entities;
using Domain.Enums;

/// <summary>
/// Tests for business rules inside the User entity.
/// No database, no Moq — just pure logic testing.
/// </summary>
public class UserEntityTests
{
    // ─────────────────────────────────────
    // IsOverdue() TESTS
    // Rule: Overdue if last immunisation > 365 days ago
    // ─────────────────────────────────────

    [Fact]
    public void IsOverdue_LastImmunisationOver365Days_ReturnsTrue()
    {
        // ARRANGE: User immunised 2 years ago
        var user = new User
        {
            Status = ImmunisationStatus.FullyImmunised,
            LastImmunisationDate = DateTime.UtcNow.AddYears(-2) // 730 days ago
        };

        // ACT & ASSERT: Should be overdue
        Assert.True(user.IsOverdue());
    }

    [Fact]
    public void IsOverdue_LastImmunisationRecent_ReturnsFalse()
    {
        // ARRANGE: User immunised 2 months ago
        var user = new User
        {
            Status = ImmunisationStatus.FullyImmunised,
            LastImmunisationDate = DateTime.UtcNow.AddMonths(-2) // ~60 days ago
        };

        // ACT & ASSERT: Should NOT be overdue
        Assert.False(user.IsOverdue());
    }

    [Fact]
    public void IsOverdue_NoImmunisationDate_ReturnsFalse()
    {
        // ARRANGE: Never immunised — like Bob in our seed data
        var user = new User
        {
            Status = ImmunisationStatus.NonImmunised,
            LastImmunisationDate = null // No date at all
        };

        // ACT & ASSERT: No date = can't determine overdue
        Assert.False(user.IsOverdue());
    }

    [Fact]
    public void IsOverdue_Exactly365Days_ReturnsFalse()
    {
        // ARRANGE: Exactly on the boundary
        var user = new User
        {
            Status = ImmunisationStatus.FullyImmunised,
            LastImmunisationDate = DateTime.UtcNow.AddDays(-365) // exactly 365
        };

        // ACT & ASSERT: Rule is > 365, so exactly 365 is NOT overdue
        Assert.False(user.IsOverdue());
    }

    [Fact]
    public void IsOverdue_366Days_ReturnsTrue()
    {
        // ARRANGE: One day past the boundary
        var user = new User
        {
            Status = ImmunisationStatus.FullyImmunised,
            LastImmunisationDate = DateTime.UtcNow.AddDays(-366) // 366 days
        };

        // ACT & ASSERT: 366 > 365, so overdue
        Assert.True(user.IsOverdue());
    }

    // ─────────────────────────────────────
    // IsFullyCompliant() TESTS
    // Rule: FullyImmunised AND NOT Overdue
    // ─────────────────────────────────────

    [Fact]
    public void IsFullyCompliant_FullyImmunisedAndNotOverdue_ReturnsTrue()
    {
        // ARRANGE: Like Charlie — recent immunisation
        var user = new User
        {
            Status = ImmunisationStatus.FullyImmunised,
            LastImmunisationDate = DateTime.UtcNow.AddMonths(-3) // 3 months ago
        };

        // ACT & ASSERT: Fully immunised + not overdue = compliant
        Assert.True(user.IsFullyCompliant());
    }

    [Fact]
    public void IsFullyCompliant_FullyImmunisedButOverdue_ReturnsFalse()
    {
        // ARRANGE: Immunised but too long ago
        var user = new User
        {
            Status = ImmunisationStatus.FullyImmunised,
            LastImmunisationDate = DateTime.UtcNow.AddYears(-2) // Overdue!
        };

        // ACT & ASSERT: Overdue cancels out FullyImmunised
        Assert.False(user.IsFullyCompliant());
    }

    [Fact]
    public void IsFullyCompliant_PartiallyImmunised_ReturnsFalse()
    {
        // ARRANGE: Like Jane — not fully immunised
        var user = new User
        {
            Status = ImmunisationStatus.PartiallyImmunised,
            LastImmunisationDate = DateTime.UtcNow.AddMonths(-1) // Recent but partial
        };

        // ACT & ASSERT: Not FullyImmunised = not compliant
        Assert.False(user.IsFullyCompliant());
    }

    [Fact]
    public void IsFullyCompliant_NonImmunised_ReturnsFalse()
    {
        // ARRANGE: Like Bob
        var user = new User
        {
            Status = ImmunisationStatus.NonImmunised,
            LastImmunisationDate = null
        };

        // ACT & ASSERT
        Assert.False(user.IsFullyCompliant());
    }

    [Fact]
    public void IsFullyCompliant_Overdue_ReturnsFalse()
    {
        // ARRANGE: Like Alice
        var user = new User
        {
            Status = ImmunisationStatus.Overdue,
            LastImmunisationDate = DateTime.UtcNow.AddYears(-2)
        };

        // ACT & ASSERT
        Assert.False(user.IsFullyCompliant());
    }
}