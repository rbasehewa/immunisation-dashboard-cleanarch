namespace Application.Interfaces;

using Application.DTOs;

public interface IDashboardService
{
    Task<DashboardStatisticsDto> GetDashboardStatisticsAsync();
    Task<IEnumerable<UserSummaryDto>> GetUserSummariesAsync();
    Task<IEnumerable<UserSummaryDto>> GetUsersByStatusAsync(string status);
}