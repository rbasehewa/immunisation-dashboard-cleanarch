namespace WebApi.Controllers;

using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]                     // Enables automatic validation, error handling
[Route("api/[controller]")]         // URL becomes: /api/dashboard
[Authorize]  
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    // DI injects these automatically
    public DashboardController(
        IDashboardService dashboardService,
        ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    /// <summary>
    /// Get immunisation dashboard statistics summary
    /// </summary>
    /// <returns>Dashboard statistics including completion rate</returns>
    [HttpGet("statistics")]                                  // GET /api/dashboard/statistics
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetStatistics()
    {
        try
        {
            _logger.LogInformation("Fetching dashboard statistics");

            var stats = await _dashboardService.GetDashboardStatisticsAsync();
            return Ok(stats);                                // Returns 200 + JSON
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard statistics");
            return StatusCode(500, "An error occurred while retrieving statistics");
        }
    }

    /// <summary>
    /// Get all user summaries
    /// </summary>
    /// <returns>List of all users with their immunisation status</returns>
    [HttpGet("users")]                                       // GET /api/dashboard/users
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            _logger.LogInformation("Fetching all user summaries");

            var users = await _dashboardService.GetUserSummariesAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user summaries");
            return StatusCode(500, "An error occurred while retrieving users");
        }
    }

    /// <summary>
    /// Get users filtered by immunisation status
    /// </summary>
    /// <param name="status">Immunisation status: NonImmunised, PartiallyImmunised, FullyImmunised, Overdue</param>
    /// <returns>Filtered list of users</returns>
    [HttpGet("users/status/{status}")]                       // GET /api/dashboard/users/status/FullyImmunised
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsersByStatus(string status)
    {
        try
        {
            _logger.LogInformation("Fetching users with status: {Status}", status);

            var users = await _dashboardService.GetUsersByStatusAsync(status);
            return Ok(users);
        }
        catch (ArgumentException ex)
        {
            // Invalid status string - return 400
            _logger.LogWarning("Invalid status requested: {Status}", status);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users by status");
            return StatusCode(500, "An error occurred");
        }
    }
}