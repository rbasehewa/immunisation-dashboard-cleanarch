namespace Domain.Entities;

using Domain.Enums;

public class User
{
    // ─────────────────────────────────────
    // DATA - what we store in the database
    // ─────────────────────────────────────
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty; // Avoid null references
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ImmunisationStatus Status { get; set; }
    public DateTime? LastImmunisationDate { get; set; } // nullable for users (some dates can be null)
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    // ─────────────────────────────────────
    // BUSINESS RULES - live here, used everywhere
    // ─────────────────────────────────────

    /// <summary>
    /// Rule: A user is overdue if their last immunisation
    /// was more than 365 days ago.
    /// </summary>
    public bool IsOverdue()
    {
        if (!LastImmunisationDate.HasValue)
            return false;

        var daysSinceLastImmunisation = (DateTime.UtcNow - LastImmunisationDate.Value).Days;

        return daysSinceLastImmunisation > 365;
    }

    /// <summary>
    /// Rule: A user is fully compliant ONLY if:
    ///   1. They are FullyImmunised AND
    ///   2. They are NOT overdue
    /// </summary>
    public bool IsFullyCompliant()
    {
        return Status == ImmunisationStatus.FullyImmunised && !IsOverdue();
    }
}