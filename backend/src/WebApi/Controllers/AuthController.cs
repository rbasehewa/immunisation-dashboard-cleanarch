namespace WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Login endpoint - returns JWT token for valid credentials.
    /// DEV ONLY: In PROD, validate against a user database.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for user: {Username}", request.Username);

        // DEV ONLY: Hardcoded credentials for testing
        // In PROD: validate against database with hashed passwords (IMPORTANT))
        if (request.Username == "admin" && request.Password == "admin123")
        {
            var token = GenerateJwtToken(request.Username);
            _logger.LogInformation("Login successful for user: {Username}", request.Username);
            
            return Ok(new LoginResponse
            {
                Token = token,
                ExpiresIn = int.Parse(_configuration["Jwt:ExpiresInHours"]!) * 3600, // Convert hours to seconds
                TokenType = "Bearer"
            });
        }

        _logger.LogWarning("Login failed for user: {Username}", request.Username);
        return Unauthorized(new { message = "Invalid username or password" });
    }

    // ─────────────────────────────────────
    // PRIVATE: Generate JWT Token
    // ─────────────────────────────────────
    private string GenerateJwtToken(string username)
    {
        // The secret key from appsettings.json
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        // Sign the token using HMAC SHA256
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Claims = data stored inside the token
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token ID
        };

        // Build the token
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:ExpiresInHours"]!)),
            signingCredentials: credentials
        );

        // Convert token to string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// ─────────────────────────────────────
// REQUEST/RESPONSE MODELS
// ─────────────────────────────────────

/// <summary>
/// Login request with username and password
/// </summary>
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Login response with JWT token
/// </summary>
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }      // Seconds until expiry
    public string TokenType { get; set; } = string.Empty;
}