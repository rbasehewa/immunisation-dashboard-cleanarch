namespace Application.Services;

using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;

public class DashboardService : IDashboardService // business logic layer
{
    private readonly IUserRepository _userRepository;

    // Constructor Injection - the repository is "injected" by DI container
    public DashboardService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<DashboardStatisticsDto> GetDashboardStatisticsAsync()
    {
        // Get raw stats from database
        var stats = await _userRepository.GetDashboardStatisticsAsync();
        
        // Business logic: Calculate completion rate
        decimal completionRate = stats.TotalUsers > 0 
            ? (decimal)stats.FullyImmunised / stats.TotalUsers * 100 
            : 0;

        // Map to DTO
        return new DashboardStatisticsDto
        {
            TotalUsers = stats.TotalUsers,
            FullyImmunised = stats.FullyImmunised,
            PartiallyImmunised = stats.PartiallyImmunised,
            NonImmunised = stats.NonImmunised,
            Overdue = stats.Overdue,
            ImmunisationCompletionRate = Math.Round(completionRate, 2)
        };
    }

    public async Task<IEnumerable<UserSummaryDto>> GetUserSummariesAsync()
    {
        var users = await _userRepository.GetAllUsersAsync();
        
        // Transform entities to DTOs
        return users.Select(u => MapToUserSummaryDto(u));
    }

    public async Task<IEnumerable<UserSummaryDto>> GetUsersByStatusAsync(string status)
    {
        // Validation: Is this a valid status?
        if (!Enum.TryParse<ImmunisationStatus>(status, true, out var immunisationStatus))
        {
            throw new ArgumentException($"Invalid immunisation status: {status}. Valid values are: {string.Join(", ", Enum.GetNames(typeof(ImmunisationStatus)))}");
        }

        var users = await _userRepository.GetUsersByStatusAsync(immunisationStatus);
        
        return users.Select(u => MapToUserSummaryDto(u));
    }

    // Helper method to avoid repeating mapping logic (DRY principle)
    private UserSummaryDto MapToUserSummaryDto(Domain.Entities.User user)
    {
        return new UserSummaryDto
        {
            Id = user.Id,
            FullName = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            Status = user.Status.ToString(),
            LastImmunisationDate = user.LastImmunisationDate,
            StatusDisplay = FormatStatusDisplay(user.Status),
            IsOverdue = user.IsOverdue(),              
            IsFullyCompliant = user.IsFullyCompliant()
        };
    }

    // Helper method to make status user-friendly (Modern switch expression)
    private string FormatStatusDisplay(ImmunisationStatus status)
    {
        return status switch
        {
            ImmunisationStatus.FullyImmunised => "Fully Immunised",
            ImmunisationStatus.PartiallyImmunised => "Partially Immunised",
            ImmunisationStatus.NonImmunised => "Not Immunised",
            ImmunisationStatus.Overdue => "Overdue",
            _ => "Unknown"
        };
    }
}