namespace Application.DTOs;

public class UserSummaryDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? LastImmunisationDate { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;

    // Domain logic fields
    public bool IsOverdue { get; set; }
    public bool IsFullyCompliant { get; set; }
}