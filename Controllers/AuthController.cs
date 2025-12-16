using CommunityEventsApi.BAL.Interfaces;
using CommunityEventsApi.DTOs.Auth;
using CommunityEventsApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunityEventsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// User login
    /// </summary>
    /// <param name="loginDto">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(HttpApiResponse<TokenResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<TokenResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(HttpApiResponse<TokenResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<TokenResponseDto>>> Login([FromBody] LoginRequestDto loginDto)
    {
        try
        {
            var result = await _authService.LoginAsync(loginDto);
            return Ok(HttpApiResponse<TokenResponseDto>.Success(result, "Login successful"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Login failed for email {Email}: {Message}", loginDto.Email, ex.Message);
            return Unauthorized(HttpApiResponse<TokenResponseDto>.Unauthorized(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email {Email}", loginDto.Email);
            return StatusCode(500, HttpApiResponse<TokenResponseDto>.InternalServerError("An error occurred during login"));
        }
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="signupDto">User registration information</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("signup")]
    [ProducesResponseType(typeof(HttpApiResponse<TokenResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(HttpApiResponse<TokenResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(HttpApiResponse<TokenResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<TokenResponseDto>>> Signup([FromBody] SignupRequestDto signupDto)
    {
        try
        {
            var result = await _authService.SignupAsync(signupDto);
            return CreatedAtAction(nameof(Signup), new HttpApiResponse<TokenResponseDto>(System.Net.HttpStatusCode.Created, "User registered successfully", result));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Signup failed: {Message}", ex.Message);
            return BadRequest(HttpApiResponse<TokenResponseDto>.BadRequest(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during signup for email {Email}", signupDto.Email);
            return StatusCode(500, HttpApiResponse<TokenResponseDto>.InternalServerError("An error occurred during registration"));
        }
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New JWT token</returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(HttpApiResponse<TokenResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<TokenResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(HttpApiResponse<TokenResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<TokenResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(HttpApiResponse<TokenResponseDto>.Success(result, "Token refreshed successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Token refresh failed: {Message}", ex.Message);
            return Unauthorized(HttpApiResponse<TokenResponseDto>.Unauthorized(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, HttpApiResponse<TokenResponseDto>.InternalServerError("An error occurred during token refresh"));
        }
    }

    /// <summary>
    /// Logout user
    /// </summary>
    /// <returns>Success message</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<object>>> Logout()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(HttpApiResponse<object>.Unauthorized("Invalid user"));
            }

            await _authService.LogoutAsync(userId);
            return Ok(new HttpApiResponse<object>(System.Net.HttpStatusCode.OK, "Logged out successfully", null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, HttpApiResponse<object>.InternalServerError("An error occurred during logout"));
        }
    }
}
